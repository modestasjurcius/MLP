using MLP.Entities.Glass;
using System.Collections.Generic;

namespace MLP.Services
{
    public class MLP
    {
        private List<Glass> _data;

        public MLP(string fileLocation)
        {
            _data = new DataReader().ReadData(fileLocation);
            _data = new Normalizer().NormalizeData(_data, -1, 1);
        }
    }
}
