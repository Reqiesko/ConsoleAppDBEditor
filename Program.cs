using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace DBLab5
{
    internal class Program
    {
        
        private static MySqlConnection Connection;
        private static DataSet dataSet;
        private static string connectionString;
        static void ShowTable(string tableName, int cols_num)
        {
            Console.WriteLine($"Автономная таблица {tableName}");
            var totalRows = dataSet.Tables[tableName].Rows.Count;
            Console.WriteLine($"Количество записей {totalRows}");
            Console.WriteLine(new string('-', 23 * cols_num));

            foreach (DataColumn column in dataSet.Tables[tableName].Columns)
            {
                Console.Write($"{column.ColumnName,-22}|");
            }
            Console.Write("|");
            Console.WriteLine();
            Console.WriteLine(new string('-', 23 * cols_num));
            foreach (DataRow row in dataSet.Tables[tableName].Rows)
            {
                foreach (DataColumn column in dataSet.Tables[tableName].Columns)
                    Console.Write($"{row[column],-22}|");

                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 23 * cols_num));
            Console.WriteLine(
                "<UP>, <DN> \t - навигация\n" +
                "<CTRL+T>   \t - вывести автономную таблицу\n" +
                "<ENTER>    \t - редактировать строку\n" +
                "<CTRL+L>   \t - вывести таблицу из БД\n" +
                "<DEL>      \t - пометить строку на удаление\n" +
                "<INSERT>   \t - добавить строку\n" +
                "<BACKSPACE>\t - отмена изменений\n" +
                "<F2>       \t - сохранить таблицу в БД\n" +
                "<ESC>      \t - выход\n");
        }

        static void ShowTableInDB(string TableName)
        {
            Console.WriteLine($"Таблица в БД: {TableName}    ");
            
            
            var myCom = new MySqlCommand();
            myCom.CommandType = CommandType.TableDirect;
            myCom.CommandText = TableName;
            try
            {
                Connection.Open();
            }
            catch
            {
                Connection.Close();
                throw;
            }

            myCom.Connection = Connection;


            var myReader = myCom.ExecuteReader();
            Console.WriteLine(new string('-', 23 * myReader.FieldCount));
            if (myReader.HasRows)
            {
                var NumberOfColumns = myReader.FieldCount;
                for (var i = 0; i < NumberOfColumns; i++)
                    Console.Write($"{myReader.GetName(i),-22}|");
            }
            Console.Write("|");
            Console.WriteLine();
            Console.WriteLine(new string('-', 23 * myReader.FieldCount));
            while (myReader.Read())
            {
                for (var i = 0; i < myReader.FieldCount; i++)
                    Console.Write($"{myReader[i],-22}|");
                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 23 * myReader.FieldCount));
            Connection.Close();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя пользователя: ");
            string name = Console.ReadLine();
            while (name.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите имя пользователя: ");
                name = Console.ReadLine();
            }
            Console.WriteLine("Введите пароль: ");
            string password = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    if (password.Length == 0)
                    {
                        Console.WriteLine("Вы ничего не ввели!");
                        Console.WriteLine("Введите пароль: ");
                    }
                    else
                        break;
                }

                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += i.KeyChar;
                    Console.Write("*");
                }
            }
            while (password.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите пароль: ");
                password = Console.ReadLine();
            }
            Console.WriteLine();
            Console.WriteLine("Введите имя сервера: ");
            string server_name = Console.ReadLine();
            while (server_name.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите имя сервера: ");
                server_name = Console.ReadLine();
            }
            Console.WriteLine("Введите имя базы данных: ");
            string db_name = Console.ReadLine();
            while (db_name.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите имя базы данных: ");
                db_name = Console.ReadLine();
            }
            connectionString = $"uid={name}; Password={password}; server={server_name}; database={db_name};";
            Connection = new MySqlConnection(connectionString);
            dataSet = new DataSet($"{db_name}");
            Console.Clear();
            var allOK = true;
            Console.WriteLine($"БД \"{db_name}\" ");
            try
            {
                Connection.Open();
            }
            catch
            {
                Console.WriteLine("Ошибка подключения к БД:");
                Console.ReadKey();
                allOK = false;
            }
            finally
            {
                Console.WriteLine("Свойство подключения:");
                Console.WriteLine($"\tСтрока подключения: {Connection.ConnectionString}");
                Console.WriteLine($"\tБаза данных: {Connection.Database}");
                Console.WriteLine($"\tСервер: {Connection.DataSource}");

                if (allOK) Console.WriteLine($"\tВерсия сервера: {Connection.ServerVersion}");
                Connection.Close();
                Console.WriteLine($"\tСостояние: {Connection.State}");
                if (!allOK) Environment.Exit(0);
            }
            Console.WriteLine("Введите имя таблицы: ");
            var TableName = Console.ReadLine();
            while (server_name.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите имя таблицы: ");
                TableName = Console.ReadLine();
            }
            Console.WriteLine("Введите название столбцов: ");
            string cols_t = Console.ReadLine();
            while (cols_t.Length == 0)
            {
                Console.WriteLine("Вы ничего не ввели!");
                Console.WriteLine("Введите название столбцов: ");
                cols_t = Console.ReadLine();
            }
            int col_num = 2;
            for (int i = 0; i < cols_t.Length; i++)
            {
                if (cols_t[i] == ' ')
                {
                    col_num++;
                }
            }
            var myCom = new MySqlCommand();
            myCom.CommandType = CommandType.TableDirect;
            myCom.CommandText = TableName;
            try
            {
                Connection.Open();
            }
            catch
            {
                Connection.Close();
                throw;
            }
            myCom.Connection = Connection;

            var myReader = myCom.ExecuteReader();
            string t_name = myReader.GetName(0);
            Connection.Close();
            if (cols_t.Contains(t_name))
            {
                col_num--;
            }
            cols_t = cols_t.Replace($"{t_name}", "");
            
            cols_t =  $"`{t_name} " + cols_t + "`";
            cols_t = cols_t.Replace("  ", " ");
            string cols = cols_t.Replace(" ", "`, `");
            var query = $"SELECT {cols} FROM {TableName}";
            var da = new MySqlDataAdapter(query, Connection);
            da.Fill(dataSet, TableName);
            string [] cols_names = new string [col_num];
            for (int i = 0; i < col_num; i++)
            {
                cols_names[i] = dataSet.Tables[TableName].Columns[i].ColumnName;
            }     
            var comBuilder = new MySqlCommandBuilder(da);
            var index = 0;
            var TotalRows = dataSet.Tables[TableName].Rows.Count;
            var drow = dataSet.Tables[TableName].Rows[index];

            ShowTable(TableName, col_num);
            Console.WriteLine("Текущая запись");
            for (int i = 0; i < col_num; i++)
            {
                Console.Write($"#{drow[cols_names[i]],10}: ");
            }
            Console.SetCursorPosition(0, Console.CursorTop);

            ConsoleKeyInfo cki;

            do
            {
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (index <= TotalRows - 1)
                        {
                            index++;
                            if (index == TotalRows)
                                index = 0;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if (index >= 0)
                        {
                            index--;
                            if (index == -1)
                                index = TotalRows - 1;
                        }
                        break;

                    case ConsoleKey.T:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            ShowTable($"{TableName}", col_num);
                        break;

                    case ConsoleKey.L:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            ShowTableInDB($"{TableName}");
                        break;
                    case ConsoleKey.Enter:
                        if (drow.RowState == DataRowState.Deleted)
                            break;
                        Console.WriteLine("\nВведите значения полей:");
                        foreach (DataColumn column in dataSet.Tables[TableName].Columns)
                        {
                            Console.Write($"{column.ColumnName}: ");
                            var val = Console.ReadLine();
                            drow[column.ColumnName] = val;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        drow.RejectChanges();
                        break;

                    case ConsoleKey.R:
                        if ((cki.Modifiers & ConsoleModifiers.Control) != 0) da.Fill(dataSet, TableName);
                        index = 0;
                        break;

                    case ConsoleKey.F2:
                        try
                        {
                            da.Update(dataSet, TableName);
                            dataSet.Tables[TableName].AcceptChanges();
                            Console.WriteLine("\nДанные сохранены!");
                        }
                        catch (DBConcurrencyException ex)
                        {
                            Console.WriteLine($"Возникло исключение: {ex.Message}");
                        }
                        TotalRows = dataSet.Tables[TableName].Rows.Count;
                        if (index != 0)
                            index--;
                        break;

                    case ConsoleKey.Delete:
                        drow.Delete();
                        break;

                    case ConsoleKey.Insert:
                        myCom = new MySqlCommand();
                        myCom.CommandType = CommandType.TableDirect;
                        myCom.CommandText = TableName;
                        try
                        {
                            Connection.Open();
                        }
                        catch
                        {
                            Connection.Close();
                            throw;
                        }
                        myCom.Connection = Connection;

                        myReader = myCom.ExecuteReader();
                        t_name = myReader.GetName(0);
                        Connection.Close();
                        Console.WriteLine(t_name);
                        myCom = new MySqlCommand();
                        myCom.CommandText = $"SELECT `{t_name}` FROM {TableName} ORDER BY `{t_name}` DESC";
                        myCom.Connection = Connection;
                        var curConState = Connection.State;
                       
                        Connection.Open();
                        var pkey = (int)myCom.ExecuteScalar();
                        Connection.Close();
                        var row = dataSet.Tables[TableName].NewRow();
                        row[0] = ++pkey;
                        dataSet.Tables[TableName].Rows.Add(row);
                        var ins_index = dataSet.Tables[TableName].Rows.IndexOf(row);
                        index = ins_index;
                        break;
                }

                TotalRows = dataSet.Tables[0].Rows.Count;
                drow = dataSet.Tables[TableName].Rows[index];
                var s = "  ";
                if (drow.RowState == DataRowState.Deleted)
                    s = $"#{drow[0, DataRowVersion.Original],10}: {drow[1, DataRowVersion.Original],-20}";
                else
                    s = $"#{drow[0],10}: {drow[1],-20} {drow.RowState.ToString(),10}";

                Console.Write(s.PadRight(30));
                Console.SetCursorPosition(0, Console.CursorTop);
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
