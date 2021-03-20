using MLP.Entities.Glass;
using MLP.Entities.Node;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MLP.Services
{
    public class DataReader
    {
        public List<Glass> ReadData(string fileLocation, List<int> idList)
        {
            var resultList = new List<Glass>();
            var isHeaderLine = true;

            using (var reader = new StreamReader(fileLocation))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (isHeaderLine)
                        isHeaderLine = false;
                    else
                    {
                        var id = int.Parse(values[0]);
                        if (idList.Contains(id))
                        {
                            var type = int.Parse(values[10]);

                            var obj = new Glass
                            {
                                Id = id,
                                RefractiveIndex = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                                Sodium = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                                Magnesium = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                                Aluminum = float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat),
                                Silicon = float.Parse(values[5], CultureInfo.InvariantCulture.NumberFormat),
                                Potassium = float.Parse(values[6], CultureInfo.InvariantCulture.NumberFormat),
                                Calcium = float.Parse(values[7], CultureInfo.InvariantCulture.NumberFormat),
                                Barium = float.Parse(values[8], CultureInfo.InvariantCulture.NumberFormat),
                                Iron = float.Parse(values[9], CultureInfo.InvariantCulture.NumberFormat),
                                Type = type,
                                TypeName = ((GlassTypeNames)type).ToString(),
                            };

                            resultList.Add(obj);
                        }
                    }
                }
            }

            return resultList;
        }

        public List<Node> ReadNodeWeightData(string fileLocation)
        {
            var resultList = new List<Node>();
            Node node = null;

            using (var reader = new StreamReader(fileLocation))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var value = values[0];

                    if (value == "SigmoidNode")
                    {
                        if (node != null)
                            resultList.Add(node);

                        node = new Node();
                        node.Name = string.Concat(values[0], values[1]);
                    }
                    else
                    {
                        if (node.Weights == null)
                            node.Weights = new List<AttributeWeight>();

                        node.Weights.Add(new AttributeWeight
                        {
                            AttributeName = values[0],
                            Value = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                        });
                    }
                }
            }

            resultList.Add(node);
            return resultList;
        }

        public List<int> ReadIdList(string fileLocation)
        {
            var results = new List<int>();

            using (var reader = new StreamReader(fileLocation))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    results.Add(int.Parse(values[0]));
                }
            }

            return results;
        }
    }
}
