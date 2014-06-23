using LINE2D;
using System;
using System.Collections.Generic;
using System.Linq;
using Range = AForge.IntRange;

namespace ParticleFilterModelFitting
{
    public static class ModelRepository
    {
        private static short[][] sortedScales;
        private static short[][] sortedAngles;

        public static Dictionary<ModelParams, ITemplate> Repository { get; private set; }

        public static void Initialize(Dictionary<ModelParams, ITemplate> repository)
        {
            Repository = repository;

            var modelGroups = from m in repository.Keys
                              group m by m.ModelTypeIndex into mGroup
                              orderby mGroup.Key
                              select mGroup;

            sortedScales = (
                              from mGroup in modelGroups
                              select (
                                     from m in mGroup
                                     orderby m.Scale ascending
                                     select m.Scale
                                     )
                                     .Distinct().ToArray()
                            )
                            .ToArray();

            sortedAngles = (
                              from mGroup in modelGroups
                              select (
                                      from m in mGroup
                                      orderby m.Angle ascending
                                      select m.Angle
                                     )
                                     .Distinct().ToArray()
                            )
                            .ToArray();

            PrototypeCount = sortedScales.Count();
            ScaleRange = new Range(sortedScales.Select(x => x.First()).Min(), sortedScales.Select(x => x.Last()).Max());
            AngleRange = new Range(sortedAngles.Select(x => x.First()).Min(), sortedAngles.Select(x => x.Last()).Max());
        }

        public static ModelParams GetClosestTo(ModelParams model)
        {
            Func<short, short, float> scaleDistFunc = (a, b) => (float)Math.Abs(a - b);
            Func<short, short, float> angleDistFunc = (a, b) => (float)Math.Abs(a - b);//(float)Angle.DistanceDeg(a, b);

            var templateIdx = model.ModelTypeIndex;
            var closestScale = getClosestValue(sortedScales[templateIdx], model.Scale, scaleDistFunc);
            var closestAngle = getClosestValue(sortedAngles[templateIdx], model.Angle, angleDistFunc);

            return new ModelParams(templateIdx, closestScale, closestAngle);
        }

        private static T getClosestValue<T>(T[] sortedCollection, T param, Func<T, T, float> distanceFunc)
        {
            int index = Array.BinarySearch(sortedCollection, param);

            if (index < 0)
            {
                // If the index is negative, it represents the bitwise 
                // complement of the next larger element in the array. 
                index = ~index;
                var indexMax = Math.Max(0, Math.Min(index, sortedCollection.Length - 1));
                var indexMin = Math.Max(0, Math.Min(index - 1, sortedCollection.Length - 1));

                if (distanceFunc(sortedCollection[indexMin], param) < distanceFunc(sortedCollection[indexMax], param))
                {
                    return sortedCollection[indexMin];
                }
                else
                {
                    return sortedCollection[indexMax];
                }
            }

            return sortedCollection[index];
        }

        public static int PrototypeCount { get; private set; }
        public static Range ScaleRange { get; private set; }
        public static Range AngleRange { get; private set; }
    }
}
