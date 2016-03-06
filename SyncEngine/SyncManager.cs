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
    public class SyncManager : ISyncManager
    {
        #region Events
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public delegate void FailedSycHandler(object sender, SyncFailedEventArgs e);
        public event FailedSycHandler OnSyncFailure;

        public delegate void CompleteSyncHandler(object sender, SyncCompleteArgs e);
        public event CompleteSyncHandler OnSyncComplete;

        public delegate void StartedSynchandler(object sender, SyncStartedArgs e);
        public event StartedSynchandler OnSyncStart;

        public delegate void Cleanuphandler(object sender, SyncCleanupArgs e);
        public event Cleanuphandler OnCleanUp;

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

        public void CreateContexts()
        {
            _remoteSessionContext = new RemoteContext(_factoryManager);
            _localSessionContext = new LocalContext(_factoryManager);
        }

        public void SyncTable_Employee()
        {
            var localTable = new EmployeeRepository<Employee>(_localSessionContext);
            var remoteTable = new EmployeeRepository<Employee>(_remoteSessionContext);

            SyncStarted("Employee");

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

                SyncProgress(++count, total);

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion
        }

        public void SyncTable_Product()
        {
            var localTable = new ProductRepository<Product>(_localSessionContext);
            var remoteTable = new ProductRepository<Product>(_remoteSessionContext);

            SyncStarted("Product");

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

                SyncProgress(++count, total);

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion

        }

        public void SyncTable_Store()
        {
            var localTable = new StoreRepository<Store>(_localSessionContext);
            var remoteTable = new StoreRepository<Store>(_remoteSessionContext);

            SyncStarted("Store");

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

                SyncProgress(++count, total);

                // Remove after successful remove
                localTable.Remove(row);
                localTable.Commit();
            }

            #endregion

        }

        public void SyncTable_Sale()
        {
            try
            {
                var localTable = new SaleRepository<Sale>(_localSessionContext);
                var remoteTable = new SaleRepository<Sale>(_remoteSessionContext);

                SyncStarted("Sale");

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

                    SyncProgress(++count, total);

                    // Remove after successful remove
                    localTable.Remove(row);
                    localTable.Commit();
                }

                #endregion
            }
            catch (Exception ex)
            {
                SyncFailed(ex);
            }

        }

        public void SyncAllTables()
        {
            try
            {
                _localSessionContext.OpenContextSession();
                _remoteSessionContext.OpenContextSession();
                _recordRepository = new SyncRecordRepository<SyncRecord>(_localSessionContext);

                SyncStarted("All Tables");

                SyncTable_Store();
                SyncTable_Employee();
                SyncTable_Product();
                SyncTable_Sale();

            }
            catch (Exception ex)
            {
                SyncFailed(ex);
            }
            finally
            {
                CleanUp_Records();
                SyncComplete();
            }
        }

        private void CleanUp_Records()
        {
            SyncCleanUp();
            _recordRepository.DeleteAll();
        }

        private void SyncCleanUp()
        {
            if (OnCleanUp == null) return;

            var cleanup = new SyncCleanupArgs("Removing sync records!");
            OnCleanUp(this, cleanup);
        }

        private void SyncFailed(Exception ex)
        {
            if (OnSyncFailure == null) return;
            
            SyncFailedEventArgs failedArgs = new SyncFailedEventArgs(ex, "Sync Failed");
            OnSyncFailure(this, failedArgs);
        }

        private void SyncProgress(int count, int total)
        {
            if (OnUpdateStatus == null) return;

            ProgressEventArgs progArgs = new ProgressEventArgs("Synced entity " + count + "/" + total);
            OnUpdateStatus(this, progArgs);
        }

        private void SyncComplete()
        {
            if (OnSyncComplete == null) return;

            SyncCompleteArgs compArgs = new SyncCompleteArgs("Sync Completed Successfully!");
            OnSyncComplete(this, compArgs);
        }

        private void SyncStarted(string table)
        {
            if (OnSyncStart == null) return;

            SyncStartedArgs startedArgs = new SyncStartedArgs("Now syncing " + table + "...");
            OnSyncStart(this, startedArgs);
        }
    }
}
