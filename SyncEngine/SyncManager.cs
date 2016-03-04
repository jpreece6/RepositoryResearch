using System;
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

        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public delegate void FailedSycHandler(object sender, SyncFailedEventArgs e);
        public event FailedSycHandler OnSyncFailure;

        public delegate void CompleteSyncHandler(object sender, SyncCompleteArgs e);
        public event CompleteSyncHandler OnSyncComplete;

        public delegate void StartedSynchandler(object sender, SyncStartedArgs e);
        public event StartedSynchandler OnSyncStart;


        private SessionContext _remoteSessionContext;
        private SessionContext _localSessionContext;

        private readonly SessionFactoryManager _factoryManager;

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
            try
            {
                var localTable = new EmployeeRepository<Employee>(_localSessionContext);
                var remoteTable = new EmployeeRepository<Employee>(_remoteSessionContext);

                SyncStarted("Employee");

                #region SyncCode

                var count = 0;
                var total = localTable.Count();
                foreach (var row in localTable.GetAll())
                {
                    // Get the id of the current object row 
                    var rowId = (int) row.GetType().GetProperty("Id", typeof (int)).GetValue(row);
                    if (remoteTable.Exists(rowId) == 0)
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

        public void SyncTable_Product()
        {
            try
            {
                var localTable = new ProductRepository<Product>(_localSessionContext);
                var remoteTable = new ProductRepository<Product>(_remoteSessionContext);

                SyncStarted("Product");

                #region SyncCode

                var count = 0;
                var total = localTable.Count();
                foreach (var row in localTable.GetAll())
                {
                    // Get the id of the current object row 
                    var rowId = (int) row.GetType().GetProperty("Id", typeof (int)).GetValue(row);
                    if (remoteTable.Exists(rowId) == 0)
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

        public void SyncTable_Store()
        {
            try
            {
                var localTable = new StoreRepository<Store>(_localSessionContext);
                var remoteTable = new StoreRepository<Store>(_remoteSessionContext);

                SyncStarted("Store");

                #region SyncCode

                var count = 0;
                var total = localTable.Count();
                foreach (var row in localTable.GetAll())
                {
                    // Get the id of the current object row 
                    var rowId = (int) row.GetType().GetProperty("Id", typeof (int)).GetValue(row);
                    if (remoteTable.Exists(rowId) == 0)
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
                    // Get the id of the current object row 
                    var rowId = (int)row.GetType().GetProperty("Id", typeof(int)).GetValue(row);
                    if (remoteTable.Exists(rowId) == 0)
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
            _localSessionContext.OpenContextSession();
            _remoteSessionContext.OpenContextSession();

            SyncStarted("All Tables");

            SyncTable_Employee();
            SyncTable_Product();
            SyncTable_Store();
            SyncTable_Sale();

            SyncComplete();
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
