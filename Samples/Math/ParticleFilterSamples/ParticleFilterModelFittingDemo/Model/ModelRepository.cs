#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Imaging.Algorithms.LINE2D;
using Range = AForge.IntRange;

namespace ParticleFilterModelFitting
{
    /// <summary>
    /// A repository for templates. Can generate template from model parameters (see <see cref="ModelParams"/> class).
    /// </summary>
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

        public static ModelParams GetMostSimilarTo(ModelParams model)
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
