using Data.SqlServer.utis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Data.SqlServer
{

    public class Database : IDisposable
    {
        internal const string SqlServerProviderName = "System.Data.SqlClient";

        private DbConnection _connection;

        public Database(DbConnection pconnection)
        {
            this._connection = pconnection;
        }

        public DbConnection Connection
        {
            get { return _connection; }
        }

        public static Database Open(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw ExceptionHelper.CreateArgumentNullOrEmptyException("connectionString");
            }
            DbProviderFactory _providerFactory = DbProviderFactories.GetFactory(SqlServerProviderName);
            DbConnection dbconn = _providerFactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            return new Database(dbconn);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Execute(string commandText, params object[] args)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw ExceptionHelper.CreateArgumentNullOrEmptyException("commandText");
            }
            this.EnsureConnectionOpen();
            DbCommand dbCommand = this.Connection.CreateCommand();
            dbCommand.CommandText = commandText;
            Database.AddParameters(dbCommand, args);
            int result;
            using (dbCommand)
            {
                result = dbCommand.ExecuteNonQuery();
            }
            return result;
        }

        public dynamic GetLastInsertId()
        {
            return this.QueryValue("SELECT @@Identity", new object[0]);
        }

        public IEnumerable<dynamic> Query(string commandText, params object[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw ExceptionHelper.CreateArgumentNullOrEmptyException("commandText");
            }
            return this.QueryInternal(commandText, parameters).ToList<object>().AsReadOnly();
        }

        public dynamic QuerySingle(string commandText, params object[] args)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw ExceptionHelper.CreateArgumentNullOrEmptyException("commandText");
            }
            return this.QueryInternal(commandText, args).FirstOrDefault<object>();
        }

        public dynamic QueryValue(string commandText, params object[] args)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw ExceptionHelper.CreateArgumentNullOrEmptyException("commandText");
            }
            this.EnsureConnectionOpen();
            DbCommand dbCommand = this.Connection.CreateCommand();
            dbCommand.CommandText = commandText;
            Database.AddParameters(dbCommand, args);
            object result;
            using (dbCommand)
            {
                result = dbCommand.ExecuteScalar();
            }
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this._connection != null)
            {
                this._connection.Close();
                this._connection = null;
            }
        }

        private static void AddParameters(DbCommand command, object[] args)
        {
            if (args == null)
            {
                return;
            }
            IEnumerable<DbParameter> enumerable = args.Select(delegate(object o, int index)
            {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = index.ToString(CultureInfo.InvariantCulture);
                dbParameter.Value = (o ?? DBNull.Value);
                return dbParameter;
            });
            foreach (DbParameter current in enumerable)
            {
                command.Parameters.Add(current);
            }
        }
        private static IEnumerable<string> GetColumnNames(DbDataRecord record)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                yield return record.GetName(i);
            }
            yield break;
        }
        private void EnsureConnectionOpen()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
               // this.OnConnectionOpened();
            }
        }
        private IEnumerable<dynamic> QueryInternal(string commandText, params object[] parameters)
        {
            this.EnsureConnectionOpen();
            DbCommand dbCommand = this.Connection.CreateCommand();
            dbCommand.CommandText = commandText;
            Database.AddParameters(dbCommand, parameters);
            using (dbCommand)
            {
                IEnumerable<string> enumerable = null;
                using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
                {
                    foreach (DbDataRecord record in dbDataReader)
                    {
                        if (enumerable == null)
                        {
                            enumerable = Database.GetColumnNames(record);
                        }
                        yield return new DynamicRecord(enumerable, record);
                    }
                }
            }
            yield break;
        }
    }
}
