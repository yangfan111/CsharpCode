using System;
using System.Collections.Generic;

namespace Core.Configuration.Equipment
{
    class EquipmentGeneration
    {
        private readonly Dictionary<int, Location> _locations;
        private readonly Dictionary<int, ProbabilisticCombination> _combinations;
        private readonly Dictionary<int, ProbabilisticPackage> _packages;
        private Random _randomizer;
        private int _genrerationId;

        public EquipmentGeneration(Dictionary<int, Location> locations,
                                   Dictionary<int, Combination> combinations,
                                   Dictionary<int, Package> packages)
        {
            _locations = locations;
            _combinations = new Dictionary<int, ProbabilisticCombination>();
            _packages = new Dictionary<int, ProbabilisticPackage>();

            InitProbabilisticPackages(packages);
            InitProbabilisticCombinations(combinations);

            _randomizer = new Random(0);
            _genrerationId = 0;
        }

        class ProbabilisticCombination
        {
            public int Id;
            public ProbabilisticPackage[] DeterminedPackages;
            public ProbabilisticPackage[] ProbabilisticPackages;
        }

        class ProbabilisticPackage
        {
            public int Id;
            public float Probability;
            public ProbabilisticResourceItem[] DeterminedResourceItems;
            public ProbabilisticResourceItem[] ProbabilisticResourceItems;
        }

        class ProbabilisticResourceItem
        {
            public int Id;
            public int Number;
            public float Probability;
        }

        private void InitProbabilisticPackages(Dictionary<int, Package> packages)
        {
            foreach (var pac in packages)
            {
                float totalWeight = 0;
                int probabilisticCount = 0;
                int determinedCount = 0;
                pac.Value.WeightedResourceItems.ForEach(
                    x =>
                    {
                        totalWeight += x.Weight;
                        if (x.Weight == 0)
                        {
                            determinedCount++;
                        }
                        else
                        {
                            probabilisticCount++;
                        }
                    });

                var probabilisticPac = new ProbabilisticPackage();
                probabilisticPac.Id = pac.Key;
                probabilisticPac.DeterminedResourceItems = new ProbabilisticResourceItem[determinedCount];
                probabilisticPac.ProbabilisticResourceItems = new ProbabilisticResourceItem[probabilisticCount];

                int currentWeight = 0;
                int d = 0, p = 0;
                foreach (var item in pac.Value.WeightedResourceItems)
                {
                    var resourceItem = new ProbabilisticResourceItem();
                    resourceItem.Id = item.Id;
                    resourceItem.Number = item.Number;

                    if (item.Weight == 0)
                    {
                        resourceItem.Probability = 1;
                        probabilisticPac.DeterminedResourceItems[d] = resourceItem;

                        d++;
                    }
                    else
                    {
                        currentWeight += item.Weight;
                        resourceItem.Probability = currentWeight / totalWeight;
                        probabilisticPac.ProbabilisticResourceItems[p] = resourceItem;

                        p++;
                    }
                }

                _packages.Add(pac.Key, probabilisticPac);
            }
        }

        private void InitProbabilisticCombinations(Dictionary<int, Combination> combinations)
        {
            foreach (var comb in combinations)
            {
                float totalWeight = 0;
                int probabilisticCount = 0;
                int determinedCount = 0;
                comb.Value.WeightedPackages.ForEach(
                    x =>
                    {
                        totalWeight += x.Weight;
                        if (x.Weight == 0)
                        {
                            determinedCount++;
                        }
                        else
                        {
                            probabilisticCount++;
                        }
                    });

                var probabilisticComb = new ProbabilisticCombination();
                probabilisticComb.Id = comb.Key;
                probabilisticComb.DeterminedPackages = new ProbabilisticPackage[determinedCount];
                probabilisticComb.ProbabilisticPackages = new ProbabilisticPackage[probabilisticCount];

                int currentWeight = 0;
                int d = 0, p = 0;
                foreach (var pac in comb.Value.WeightedPackages)
                {
                    var probPac = new ProbabilisticPackage();
                    probPac.Id = pac.Id;
                    probPac.DeterminedResourceItems = _packages[pac.Id].DeterminedResourceItems;
                    probPac.ProbabilisticResourceItems = _packages[pac.Id].ProbabilisticResourceItems;

                    if (pac.Weight == 0)
                    {
                        probPac.Probability = 1;
                        probabilisticComb.DeterminedPackages[d] = probPac;

                        d++;
                    }
                    else
                    {
                        currentWeight += pac.Weight;
                        probPac.Probability = currentWeight / totalWeight;
                        probabilisticComb.ProbabilisticPackages[p] = probPac;

                        p++;
                    }
                }

                _combinations.Add(comb.Key, probabilisticComb);
            }
        }

        public Dictionary<int, DeterminedLocation> Generate()
        {
            Dictionary<int, DeterminedLocation> ret = new Dictionary<int, DeterminedLocation>();

            foreach (var loc in _locations)
            {
                Dictionary<int, DeterminedResourceItem> items = new Dictionary<int, DeterminedResourceItem>();

                var combination = _combinations[loc.Value.CombinationId];
                CombinationGeneration(combination, items);

                ret.Add(loc.Key, new DeterminedLocation { Id = loc.Key, ResourceItems = items });
            }

            return ret;
        }

        private void CombinationGeneration(ProbabilisticCombination combination, Dictionary<int, DeterminedResourceItem> result)
        {
            foreach (var pac in combination.DeterminedPackages)
            {
                PackageGeneration(pac, result);
            }

            double r = _randomizer.NextDouble();
            foreach (var pac in combination.ProbabilisticPackages)
            {
                if (r < pac.Probability)
                {
                    PackageGeneration(pac, result);
                    break;
                }
            }
        }

        private void PackageGeneration(ProbabilisticPackage package, Dictionary<int ,DeterminedResourceItem> result)
        {
            foreach (var item in package.DeterminedResourceItems)
            {
                var id = _genrerationId++;
                result.Add(id, new DeterminedResourceItem { Id = id, ResourceItemId = item.Id, Number = item.Number });
            }

            double r = _randomizer.NextDouble();
            foreach (var item in package.ProbabilisticResourceItems)
            {
                if (r < item.Probability)
                {
                    var id = _genrerationId++;
                    result.Add(id, new DeterminedResourceItem { Id = id, ResourceItemId = item.Id, Number = item.Number });
                    break;
                }
            }
        }
    }
}
