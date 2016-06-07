The MSSQL persistence tests require the setup of a SQL Server LocalDB named v12.0. 

Create this as the principal which is running tests by starting an administrator command prompt
and typing:

runas /u:<username> "C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe" create "12.0" -s

To explore the post-test database, use SQL Server Management Studio, connect to (LocalDB)\V12.0 and expand
OpenIZ_Test. 