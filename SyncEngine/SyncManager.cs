using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using DataEngine;
using DataEngine.Contexts;
using DataEngine.Entities;
using Helpers;
using SyncEngine.Contexts;
using SyncEngine.Events;

namespace SyncEngine
{
    // TODO TALK ABOUT GENERIC ISSUES WHERE YOU CANT HAVE DIFFERENT TYPES EVEN INTERFACES
    // TODO - Note the addition of sync records to update references
    // TODO - talk about possibly syncing all data down then back up
    public class SyncManager
    {
        #region Events
        public event EventHandler<ProgressEventArgs> OnUpdateStatus;
        public event EventHandler<SyncFailedEventArgs> OnSyncFailure;
        public event EventHandler<SyncCompleteArgs> OnSyncComplete;
        public event EventHandler<SyncStartedArgs> OnSyncStart;
        public event EventHandler<SyncCleanupArgs> OnCleanUp;
        #endregion


        private SessionContext _remoteSessionContext;
        private SessionContext _localSessionContext;

        private readonly SessionFactoryManager _factoryManager;
        private SyncRecordRepository<SyncRecord> _recordRepository; 

        public SyncManager()
        {
            _factoryManager = new SessionFactoryManager();

            CreateContexts();
        }

        private void CreateContexts()
        {
            _remoteSessionContext = new RemoteContext(_factoryManager);
            _localSessionContext = new LocalContext(_factoryManager);
        }

        private void SyncTable_Employee()
        {
            var localTable = new EmployeeRepository<Employee>(_localSessionContext);
            var remoteTable = new EmployeeRepository<Employee>(_remoteSessionContext);

            OnSyncStart?.Invoke(this, new SyncStartedArgs("Employee"));

            #region SyncCode

            var count = 0;
            var total = localTable.Count();
            foreach (var row in localTable.GetAll())
            {
                // Find a sync record that matches our related column
                var record = _recordRepository.GetByLocalId(row.StoreId, "store_table");

                // Update our related column with the remote ID to prevent reference errors
                if (record != null)
                    row.StoreId = record.Remote_Id;

                if (remoteTable.Exists(row.Id) == 0)
                {
                    var newObject = ObjectCopy.Copy(row);
                    remoteTable.Save(newObject);
                }
                else
                {
                    remoteTable.Save(row);
                }

                // Save changes to remote
                remoteTable.Commit();

                OnUpdateStatus?.Invoke(this, new ProgressEventArgs("Synced entity " + ++count + "/" + total));

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion
        }

        private void SyncTable_Product()
        {
            var localTable = new ProductRepository<Product>(_localSessionContext);
            var remoteTable = new ProductRepository<Product>(_remoteSessionContext);

            OnSyncStart?.Invoke(this, new SyncStartedArgs("Product"));

            #region SyncCode

            var count = 0;
            var total = localTable.Count();
            foreach (var row in localTable.GetAll())
            {

                var remoteId = 0;
                if (remoteTable.Exists(row.Id) == 0)
                {
                    var newObject = ObjectCopy.Copy(row);
                    remoteTable.Save(newObject);
                    remoteId = 0;
                }
                else
                {
                    remoteTable.Save(row);
                }

                // Save changes to remote
                remoteTable.Commit();

                // Create a sync record this captures the remote ID value
                // so we can update any relationships...
                var record = new SyncRecord();
                record.Table_Id = "product_table";
                record.Local_Id = row.Id;
                record.Remote_Id = remoteId;
                _recordRepository.Save(record);
                _recordRepository.Commit();

                OnUpdateStatus?.Invoke(this, new ProgressEventArgs("Synced entity " + ++count + "/" + total));

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion

        }

        private void SyncTable_Store()
        {
            var localTable = new StoreRepository<Store>(_localSessionContext);
            var remoteTable = new StoreRepository<Store>(_remoteSessionContext);

            OnSyncStart?.Invoke(this, new SyncStartedArgs("Store"));

            #region SyncCode

            var count = 0;
            var total = localTable.Count();
            foreach (var row in localTable.GetAll())
            {
                var remoteId = 0;
                if (remoteTable.Exists(row.Id) == 0) // TODO REMOVE!
                {
                    var newObject = ObjectCopy.Copy(row);
                    remoteTable.Save(newObject);
                    remoteId = newObject.Id;
                }
                else
                {
                    remoteTable.Save(row);
                }

                // Save changes to remote
                remoteTable.Commit();

                // Create a sync record this captures the remote ID value
                // so we can update any relationships...
                var record = new SyncRecord();
                record.Table_Id = "store_table";
                record.Local_Id = row.Id;
                record.Remote_Id = remoteId;
                _recordRepository.Save(record);
                _recordRepository.Commit();

                OnUpdateStatus?.Invoke(this, new ProgressEventArgs("Synced entity " + ++count + "/" + total));

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion
        }

        private void SyncTable_Sale()
        {
            var localTable = new SaleRepository<Sale>(_localSessionContext);
            var remoteTable = new SaleRepository<Sale>(_remoteSessionContext);

            OnSyncStart?.Invoke(this, new SyncStartedArgs("Sale"));

            #region SyncCode

            var count = 0;
            var total = localTable.Count();
            foreach (var row in localTable.GetAll())
            {
                // Find a sync record that matches our related column
                var recordProduct = _recordRepository.GetByLocalId(row.ProductId, "product_table");
                var recordStore = _recordRepository.GetByLocalId(row.StoreId, "store_table");

                // Update our related column with the remote ID to prevent reference errors
                // Update product relation
                if (recordProduct != null)
                    row.ProductId = recordProduct.Remote_Id;

                // Update store relation
                if (recordStore != null)
                    row.StoreId = recordStore.Remote_Id;

                if (remoteTable.Exists(row.Id) == 0)
                {
                    var newObject = ObjectCopy.Copy(row);
                    remoteTable.Save(newObject);
                }
                else
                {
                    remoteTable.Save(row);
                }

                // Save changes to remote
                remoteTable.Commit();

                OnUpdateStatus?.Invoke(this, new ProgressEventArgs("Synced entity " + ++count + "/" + total));

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion

        }

        public void SyncAllTables()
        {
            try
            {
                CreateContexts();

                _localSessionContext.OpenContextSession();
                _remoteSessionContext.OpenContextSession();

                if (_remoteSessionContext.IsLocal())
                {
                    OnSyncFailure?.Invoke(this, new SyncFailedEventArgs(new Exception("Remote server is unavilable!"), "Unable to sync at this time!"));
                    return;
                }

                _recordRepository = new SyncRecordRepository<SyncRecord>(_localSessionContext);

                OnSyncStart?.Invoke(this, new SyncStartedArgs("All Tables"));

                SyncTable_Store();
                SyncTable_Employee();
                SyncTable_Product();
                SyncTable_Sale();

            }
            catch (Exception ex)
            {
                OnSyncFailure?.Invoke(this, new SyncFailedEventArgs(ex, "Sync Failed"));
            }
            finally
            {
                if (_remoteSessionContext.IsLocal() == false)
                {
                    CleanUp_Records();
                    OnSyncComplete?.Invoke(this, new SyncCompleteArgs("Sync Completed Successfully!"));
                }
            }
        }

        private void CleanUp_Records()
        {
            OnCleanUp?.Invoke(this, new SyncCleanupArgs("Removing sync records!"));
            _recordRepository.DeleteAll();
        }
    }
}
