using MLP.Entities.Glass;
using MLP.Entities.Node;
using MLP.Entities.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MLP.Services
{
    public class MLP
    {
        private List<Glass> _data;
        private List<int> _testIdList;

        private List<Node> _firstLayerNodes;
        private List<Node> _secondLayerNodes;
        private List<Node> _outputLayerNodes;

        private List<LayerValues> _firstLayerValues;
        private List<LayerValues> _secondLayerValues;
        private List<LayerValues> _outputLayerValues;

        private List<Results> _resultList;

        public MLP()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            var dataReader = new DataReader();
            _testIdList = dataReader.ReadIdList(string.Concat(projectDirectory, "\\Data\\test_ids.data"));

            _data = dataReader.ReadData(string.Concat(projectDirectory, "\\Data\\glass.data"));
            _data = new Normalizer().NormalizeData(_data, -1, 1);

            _data = _data.Where(d => _testIdList.Contains(d.Id)).ToList();

            _firstLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\input_weights.data"));
            _secondLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\hidden_layer_weights.data"));
            _outputLayerNodes = dataReader.ReadNodeWeightData(string.Concat(projectDirectory, "\\Data\\output.data"));

            _firstLayerValues = CalculateFirstLayer();
            _secondLayerValues = CalculateSecondLayer();
            _outputLayerValues = CalculateOutput();

            _resultList = GenerateResults();
            dataReader.WriteResults(_resultList, string.Concat(projectDirectory, "\\Data\\results.data"));
        }

        private List<LayerValues> CalculateFirstLayer()
        {
            var results = SeedDataFromNodes(_firstLayerNodes);

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
            var results = SeedDataFromNodes(_secondLayerNodes);

            foreach (var node in _secondLayerNodes)
            {
                for (int i = 0; i < _firstLayerValues.FirstOrDefault().Values.Count; i++)
                {
                    float sum = 0;

                    foreach (var attr in node.Weights)
                    {
                        var dataValues = _firstLayerValues.Find(v => v.Name == string.Concat("Sigmoid", attr.AttributeName, ".a"));
                        if (dataValues == null)
                        {
                            //threshold
                            sum += attr.Value;
                        }
                        else
                        {
                            var value = dataValues.Values[i];
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

        private List<LayerValues> CalculateOutput()
        {
            var results = SeedDataFromNodes(_outputLayerNodes);

            foreach (var node in _outputLayerNodes)
            {
                for (int i = 0; i < _secondLayerValues.FirstOrDefault().Values.Count; i++)
                {
                    float sum = 0;

                    foreach (var attr in node.Weights)
                    {
                        var dataValues = _secondLayerValues.Find(v => v.Name == string.Concat("Sigmoid", attr.AttributeName, ".a"));
                        if (dataValues == null)
                        {
                            //threshold
                            sum += attr.Value;
                        }
                        else
                        {
                            var value = dataValues.Values[i];
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

        public List<Results> GenerateResults()
        {
            var results = new List<Results>();

            var templateObj = _outputLayerValues.FirstOrDefault();

            int index = 0;
            foreach (var temp in templateObj.Values)
            {
                float max = 0;
                int maxId = 0;
                foreach (var value in _outputLayerValues)
                {
                    var val = value.Values[index];
                    if (val > max)
                    {
                        max = val;

                        var maxNodeName = value.Name.Replace("SigmoidNode", "").Replace(".a", "");
                        maxId = int.Parse(maxNodeName);
                    }
                }

                if (maxId < 3)
                    maxId++;
                else
                    maxId += 2;


                var originalDataObject = _data[index];
                var res = new Results();
                res.DataId = originalDataObject.Id;
                res.Original = originalDataObject.TypeName;
                res.Predicted = ((GlassTypeNames)maxId).ToString();

                results.Add(res);
                index++;
            }

            return results;
        }

        private List<LayerValues> SeedDataFromNodes(List<Node> nodes)
        {
            var results = new List<LayerValues>();

            foreach (var node in nodes)
            {
                results.Add(new LayerValues
                {
                    Name = string.Concat(node.Name, ".a"),
                    Values = new List<float>(),
                });
            }

            return results;
        }
    }
}
