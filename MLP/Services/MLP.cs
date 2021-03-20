using MLP.Entities.Glass;
using MLP.Entities.Node;
using System;
using System.Collections.Generic;
using System.IO;

namespace MLP.Services
{
    public class MLP
    {
        private List<Glass> _data;
        private List<Node> _firstLayerNodes;
        private List<Node> _secondLayerNodes;

        public MLP()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            var dataReader = new DataReader();
            _data = dataReader.ReadData(string.Concat(projectDirectory, "\\Data\\glass.data"));
            _data = new Normalizer().NormalizeData(_data, -1, 1);

            _firstLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\input_weights.data"));
            _secondLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\hidden_layer_weights.data"));
        }
    }
}
