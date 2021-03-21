using MLP.Entities.Glass;
using MLP.Entities.Node;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MLP.Services
{
    public class MLP
    {
        private List<Glass> _data;
        private List<Node> _firstLayerNodes;
        private List<Node> _secondLayerNodes;
        private List<int> _testIdList;

        private List<LayerValues> _firstLayerValues;
        private List<LayerValues> _secondLayerValues;

        public MLP()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            var dataReader = new DataReader();
            _testIdList = dataReader.ReadIdList(string.Concat(projectDirectory, "\\Data\\test_ids.data"));

            _data = dataReader.ReadData(string.Concat(projectDirectory, "\\Data\\glass.data"), _testIdList);
            _data = new Normalizer().NormalizeData(_data, -1, 1);

            _firstLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\input_weights.data"));
            _secondLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\hidden_layer_weights.data"));

            _firstLayerValues = CalculateFirstLayer();
            _secondLayerValues = CalculateSecondLayer();
        }

        private List<LayerValues> CalculateFirstLayer()
        {
            var results = new List<LayerValues>();

            //seed
            foreach (var node in _firstLayerNodes)
            {
                results.Add(new LayerValues
                {
                    Name = string.Concat(node.Name, ".a"),
                    Values = new List<float>(),
                });
            }

            //calculate a
            foreach (var data in _data)
            {
                foreach (var node in _firstLayerNodes)
                {
                    float sum = 0;

                    foreach (var attr in node.Weights)
                    {
                        var objectProperty = data.GetType().GetProperty(attr.AttributeName);
                        if (objectProperty == null)
                        {
                            //threshold
                            sum += attr.Value;
                        }
                        else
                        {
                            var value = (float)objectProperty.GetValue(data, null);
                            sum += (value * attr.Value);
                        }
                    }

                    var resultValue = results.Find(r => r.Name == string.Concat(node.Name, ".a"));

                    //calculate f(a)
                    float final = (float)(1 / (1 + Math.Exp(-sum)));
                    resultValue.Values.Add(final);
                }
            }

            return results;
        }

        private List<LayerValues> CalculateSecondLayer()
        {
            var results = new List<LayerValues>();

            //seed
            foreach (var node in _secondLayerNodes)
            {
                results.Add(new LayerValues
                {
                    Name = string.Concat(node.Name, ".a"),
                    Values = new List<float>(),
                });
            }


            foreach (var node in _secondLayerNodes)
            {
                for (int i = 0; i < _firstLayerValues.FirstOrDefault().Values.Count; i++)
                {
                    float sum = 0;

                    foreach (var attr in node.Weights)
                    {
                        //attr.Name = "Node1"
                        //data.Name = "SigmoidNode1.a"

                        var dataVals = _firstLayerValues.Find(v => v.Name == string.Concat("Sigmoid", attr.AttributeName, ".a"));
                        if (dataVals == null)
                        {
                            //threshold
                            sum += attr.Value;
                        }
                        else
                        {
                            //(float)objectProperty.GetValue(data, null);
                            var value = dataVals.Values[i];
                            sum += value * attr.Value;
                        }
                    }

                    //calc f(a)
                    float final = (float)(1 / (1 + Math.Exp(-sum)));

                    var resultValue = results.Find(r => r.Name == string.Concat(node.Name, ".a"));
                    resultValue.Values.Add(final);
                }
            }

            return results;
        }
    }
}
