using MLP.Entities.Glass;
using MLP.Entities.Normalizer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MLP.Services
{
    public class Normalizer
    {
        public List<Glass> NormalizeData(List<Glass> data, float min, float max)
        {
            var boundaries = GetBoundaries(data);

            foreach (var obj in data)
            {
                foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                {
                    if (propertyInfo.PropertyType == typeof(float))
                    {
                        var value = (float)obj.GetType().GetProperty(propertyInfo.Name).GetValue(obj, null);
                        var boundary = boundaries.Find(b => b.PropertyName == propertyInfo.Name);
                        float normalizedValue = min + (((value - boundary.Min) * (max - min)) / (boundary.Max - boundary.Min));

                        propertyInfo.SetValue(obj, normalizedValue);
                    }
                }
            }

            return data;
        }

        private List<FloatDataBoundary> GetBoundaries(List<Glass> data)
        {
            var resultList = new List<FloatDataBoundary>();
            var templateObj = data.FirstOrDefault();

            foreach (PropertyInfo propertyInfo in templateObj.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(float))
                {
                    var boundary = GetBoundary(data, propertyInfo.Name);
                    resultList.Add(boundary);
                }
            }

            return resultList;
        }

        private FloatDataBoundary GetBoundary(List<Glass> data, string propertyName)
        {
            var boundary = new FloatDataBoundary();
            boundary.PropertyName = propertyName;

            float? min = null;
            float? max = null;

            foreach (var obj in data)
            {
                var value = (float)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                if (!min.HasValue || !max.HasValue)
                {
                    min = value;
                    max = value;
                }
                else
                {
                    if (value < min)
                        min = value;
                    if (value > max)
                        max = value;
                }
            }

            boundary.Max = max.Value;
            boundary.Min = min.Value;

            return boundary;
        }
    }
}
