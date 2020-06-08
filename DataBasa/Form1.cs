using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;//  работа с файлами. позволяет не писать System.IO.
using System.Runtime.Serialization.Formatters.Binary;// для сериализации

namespace DataBasa
{
    public partial class Form1 : Form
    {
        static int sizeArr;// размер массива
        Basa[] arr;// динамическая база данных 

        // создаем объект BinaryFormatter для сериализации
        BinaryFormatter formatter = new BinaryFormatter();


        public Form1()
        {
            InitializeComponent();

            // Text files(*.txt) меню в выпадающем списке. *.txt - ищет файлы с этим расширением
            openFileDialog.Filter = "Text files(*.bas)|*.bas|All files(*.*)|*.*";
            saveFileDialog.Filter = "Text files(*.bas)|*.bas|All files(*.*)|*.*";

        }


        private void printInfo()// вывод массива объектов
        {
            if (sizeArr == 0)// если маcсив 0 то выход
                return;

            exelConsole.RowCount = sizeArr;// создаем таблицу на размер массива

            for (int i = 0; i < sizeArr; i++)// x y z (противоположен массиву)
            {
                // колонки 
                exelConsole[0, i].Value = arr[i].ID;// ID
                exelConsole[1, i].Value = arr[i].Year;// Год
                exelConsole[2, i].Value = arr[i].NumberOfCar;// Номер
                exelConsole[3, i].Value = arr[i].ColourOfCar;// Цвет
                exelConsole[4, i].Value = arr[i].MarkOfCar;// Марка
                exelConsole[5, i].Value = arr[i].FullName;// ФИО
                exelConsole[6, i].Value = arr[i].Address;// Адрес
            }                                   
        }


        private void buttonOpenFile_Click(object sender, EventArgs e)// Upload Info File
        {
            // ShowDialog() показывает диалоговое окно
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            // получаем путь к файлу
            string filename = openFileDialog.FileName;


            try
            {
                // вариант альтернативный вариант но из-за PeekChar() бывают ошибки  "god save you"
                //using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open))) // создаем объект BinaryReader
                //{
                //    // создание мссива 
                //    sizeArr = reader.ReadInt32();// получаем размер массива 
                //    //MessageBox.Show("" + sizeArr);
                //    arr = new Class1[sizeArr];

                //    for (int i = 0; i < sizeArr; i++)
                //        arr[i] = new Class1();

                //    dataGridView1.RowCount = sizeArr;// создаем кол-во строк для данных
                //    // пока не достигнут конец файла
                //    // считываем каждое значение из файла


                //    for (int i = 0; reader.PeekChar() > -1; i++)
                //    {
                //        arr[i].A1 = reader.ReadInt32();
                //        arr[i].A2 = reader.ReadInt32();
                //        arr[i].Str1 = reader.ReadString();
                //        arr[i].Str2 = reader.ReadString();
                //    }
                //}

                // десериализация из файла 
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    sizeArr = (int)formatter.Deserialize(fs);// получем размер массива 


                    // создаем массив объектов
                    arr = new Basa[sizeArr];
                    for (int i = 0; i < sizeArr; i++)
                        arr[i] = new Basa();

                    // считываем данные из файла
                    for (int i = 0; i < sizeArr; i++)
                        arr[i] = (Basa)formatter.Deserialize(fs);
                }

                MessageBox.Show("Файл загружен. Размер: " + sizeArr);
                printInfo();// вывод инфы в exel
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка чтения");
            }
        }

        private void buttonSaveFile_Click(object sender, EventArgs e)// Save Info in File
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            // получаем выбранный файл
            string filename = saveFileDialog.FileName;

            try
            {
                // альтернативный способ записи в файл
                //using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))// создаем объект BinaryWriter
                //{
                //    writer.Write(sizeArr);// запись размера массива в файл

                //    // запись объектов в файл
                //    foreach (Class1 s in arr)
                //    {
                //        writer.Write(s.A1);
                //        writer.Write(s.A2);
                //        writer.Write(s.Str1);
                //        writer.Write(s.Str2);
                //    }
                //}


                //получаем поток, куда будем записывать сериализованный объект
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, sizeArr);// сохраняем размер массива

                    // сереализация переводит в поток 1 объект для массива нужно использовать функцию
                    for (int i = 0; i < sizeArr; i++)
                        formatter.Serialize(fs, arr[i]);// сиреалезуем объекты в поток а поток в файл

                    MessageBox.Show("Файл сохранен");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Ошибка записи.");
            }


        }

        private void buttonApply_Click(object sender, EventArgs e)// Apply
        {
            // создание масива  на размер таблицы (позже прога скоректирует размер до правельного)     
            if (sizeArr < exelConsole.RowCount)// получаем его размернось
                sizeArr = exelConsole.RowCount - 1;// -1 т.к. dataGridView1.RowCount - больше на 1 чем наш массив

            if (sizeArr == 0)// если sizeArr = 0 то выходим из программы
                return;

            // создадим ссылку на массив (инициализируем ниже)
            arr = new Basa[sizeArr];
           

            int indexRightObject = 0;// index что бы заполнять arr без пропусков (от неудачно считанных)
            int unreadObjects = 0;// кол-во НЕ считаных объектов
            for (int i = 0; i < sizeArr; i++)
            {
                Basa temp = new Basa();/// ///////////////////////////////////////////////////////////////////////////////////////////////////////// протестить 

                try
                {
                    // пробуем считать
                    temp.ID =   Convert.ToInt32(exelConsole[0, i].Value);// ID
                    temp.Year = Convert.ToInt32(exelConsole[1, i].Value);// Год
                    temp.NumberOfCar =  exelConsole[2, i].Value.ToString();// Номер
                    temp.ColourOfCar =  exelConsole[3, i].Value.ToString();// Цвет
                    temp.MarkOfCar =    exelConsole[4, i].Value.ToString();// Марка
                    temp.FullName =     exelConsole[5, i].Value.ToString();// ФИО
                    temp.Address =      exelConsole[6, i].Value.ToString();// Адрес


                    // если объект успешно считан то пихаем в массив
                    arr[indexRightObject] = temp;
                    indexRightObject++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    unreadObjects++;
                }
            }

            if (unreadObjects > 0)// если есть непрочитанные объекты 
            {
                sizeArr -= unreadObjects;// удаляем хвост пустых элементов в arr
                Array.Resize(ref arr, sizeArr);
            }

            MessageBox.Show("Cохранено:" + sizeArr + "\nНесохраненных: " + unreadObjects);
            printInfo();// вывод всей сохраненной инфы
        }

        private void buttonSearch_Click(object sender, EventArgs e)// Search
        {
            // получаем информацию из стоки поиска
            string searchInfo = searchBox.Text;

            // проверка если инфы нет
            if (string.IsNullOrEmpty(searchInfo))
            {
                MessageBox.Show("Строка пуста.");
                return;
            }

            bool isFound = false;// если какая-то информация была найденна 
            for (int i = 0; i < sizeArr; i++)
            {
                // преобразуем типы даннх в string для .IndexOf(searchInfo)
                string temp1 = "" + arr[i].ID;// .ToString не сработал // 
                string temp2 = "" + arr[i].Year;


                // IndexOf() вернет -1 если ничего не найдено
                // поиск информации в строках
                if (temp1.IndexOf(searchInfo) > -1 ||
                    temp2.IndexOf(searchInfo) > -1 ||
                     
                    arr[i].NumberOfCar.IndexOf(searchInfo) > -1 ||
                    arr[i].ColourOfCar.IndexOf(searchInfo) > -1 ||
                    arr[i].MarkOfCar.IndexOf(searchInfo) > -1 ||
                    arr[i].FullName.IndexOf(searchInfo) > -1 ||
                    arr[i].Address.IndexOf(searchInfo) > -1)
                {
                    if (isFound == false) exelConsole.Rows.Clear();// очистка таблици 1 раз

                    exelConsole.Rows.Add(arr[i].ID, arr[i].Year, arr[i].NumberOfCar, arr[i].ColourOfCar, arr[i].MarkOfCar, arr[i].FullName, arr[i].Address);// добавим инфу в exel если что-то совпало
                   
                    isFound = true;
                }
            }

            if (isFound) MessageBox.Show("Найденная информация.");
            else MessageBox.Show("Ничего не найдено");
        }

        private void buttonRestart_Click(object sender, EventArgs e)// Reboot app
        {
            Application.Restart();
        }

        private void buttonShowInfo_Click(object sender, EventArgs e)// Show
        {
            printInfo();
        }

        private void buttonFAQ_Click(object sender, EventArgs e)// FAQ
        {
            System.Diagnostics.Process.Start("FAQ_Info.html");// запускает локальный файл и инструкцией 
        }

    }
}
