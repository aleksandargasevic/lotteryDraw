using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryDraw.Data
{
    class PeopleRepository:IPeopleRespository
    {
        public ObservableCollection<string> GetPoeple()
        {
            ObservableCollection<string> peoples = new ObservableCollection<string>();

            for (int i = 0; i < 20; i++)
            {
                var people = "Ime je " + i;
                peoples.Add(people);
            }

            return peoples;
        }
    }
}
