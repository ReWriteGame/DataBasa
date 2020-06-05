using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBasa
{
    [Serializable] // добавляем возможность сериализации 
    class Basa
    {
        // быстро инкапсулировать alt+enter
        private int id;
        private int year;
        private string numberOfCar;
        private string colourOfCar;
        private string markOfCar;
        private string fullName;
        private string address;

        public int ID { get => id; set => id = value; }
        public int Year { get => year; set => year = value; }
        public string NumberOfCar { get => numberOfCar; set => numberOfCar = value; }
        public string ColourOfCar { get => colourOfCar; set => colourOfCar = value; }
        public string MarkOfCar { get => markOfCar; set => markOfCar = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string Address { get => address; set => address = value; }


        public Basa()
        {
            id = 0;
            year = 0;
            numberOfCar = "";
            colourOfCar = "";
            markOfCar = "";
            fullName = "";
            address = "";
        }
    }
}
