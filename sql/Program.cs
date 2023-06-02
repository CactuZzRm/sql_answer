using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sql_bakery_app
{
    class Program
    {
        static string connectionString = "Data Source=127.0.0.1;User Id=root;Password=123456";

        static void Create(string query)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);

            command.ExecuteNonQuery();
        }

        static void CreateBDandTables()
        {
            //Создание БД 
            Create("CREATE SCHEMA `bakery`");        

            // Создание таблицы 1 
            Create(@"CREATE TABLE `bakery`.`product` (
  `id` INT NOT NULL,
  `name` VARCHAR(45) NOT NULL,
  `cost` INT NOT NULL,
  PRIMARY KEY (`id`))");
            // Создание таблицы 2 
            Create(@"CREATE TABLE `bakery`.`buyer` (
  `id` INT NOT NULL,
  `FIO` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))");
            // создадние таблицы 3 
            Create(@"CREATE TABLE `bakery`.`sells` (
  `id` INT NOT NULL,
  `product_id` INT NOT NULL,
  `buyer_id` INT NOT NULL,
  `count` INT NOT NULL,
  PRIMARY KEY (`id`))");

            // добавление связи 
            Create(@"ALTER TABLE `bakery`.`sells` 
ADD INDEX `buyer_id_idx` (`buyer_id` ASC) VISIBLE,
ADD INDEX `product_id_idx` (`product_id` ASC) VISIBLE;

ALTER TABLE `bakery`.`sells` 
ADD CONSTRAINT `buyer_id`
  FOREIGN KEY (`buyer_id`)
  REFERENCES `bakery`.`buyer` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `product_id`
  FOREIGN KEY (`product_id`)
  REFERENCES `bakery`.`product` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

");
        }

        static void AddDataToTables()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"INSERT INTO `bakery`.`buyer` (`id`, `FIO`) VALUES ('1', 'Вася ');
INSERT INTO `bakery`.`buyer` (`id`, `FIO`) VALUES ('2', 'Петя'); 
 
INSERT INTO `bakery`.`product` (`id`, `name`, `cost`) VALUES ('1', 'Булка', '15');
INSERT INTO `bakery`.`product` (`id`, `name`, `cost`) VALUES ('2', 'Кекс', '20');
 
INSERT INTO `bakery`.`sells` (`id`, `product_id`, `buyer_id`, `count`) VALUES ('1', '1', '1', '1');
INSERT INTO `bakery`.`sells` (`id`, `product_id`, `buyer_id`, `count`) VALUES ('2', '2', '2', '2');", connection);

            command.ExecuteNonQuery();
        }

        static void Update()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"
            UPDATE bakery.sells
set product_id = 2, buyer_id = 2
where bakery.sells.id = 1;", connection);

            command.ExecuteNonQuery();
        }

        static List<String> PrintSells()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"SELECT 
s.id as 'id заказа', 
p.name as 'Название продукта', 
b.FIO, s.count as 'Кол-во', 
s.count * p.cost as 'Сумма покупки' 
FROM bakery.sells as s 
join bakery.buyer as b on b.id = s.buyer_id
join bakery.product as p on s.product_id = p.id", connection);

            var c = command.ExecuteReader();

            List<string> result = new List<string>();

            while (c.Read())
            {
                object id = c.GetValue(0);
                object productName = c.GetValue(1);
                object FIO = c.GetValue(2);
                object count = c.GetValue(3);
                object sum = c.GetValue(3);

                string row = string.Format("{0}, {1}, {2}, {3}", id, productName, FIO, count, sum);

                result.Add(row);
            }

            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                
                while (true)
                {
                    Console.WriteLine("Выберете действие:");
                    Console.WriteLine("1. Создать БД и таблицы");
                    Console.WriteLine("2. Добавить элементы в таблицу");
                    Console.WriteLine("3. Отобразить содержимое таблицы sells");
                    Console.WriteLine("4. Update");
                    int n = Convert.ToInt32(Console.ReadLine());

                    if (n == 1)
                    {
                        CreateBDandTables();
                        continue;
                    }
                    else if (n == 2)
                    {
                        AddDataToTables();
                        continue;
                    }
                    else if (n == 3)
                    {
                        List<String> res = PrintSells();
                        foreach (var row in res)
                        {
                            Console.WriteLine(row);
                        }
                    }
                    else if (n == 4)
                    {
                        Update();
                    }
                    else
                    {
                        Console.WriteLine("неверный ввод");
                        continue;
                    }
                    Console.WriteLine("");
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
                Console.WriteLine("Подключение закрыто");
            }
        }
    }
}