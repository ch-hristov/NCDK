/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
 *               2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Config;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Formula
{
    /// <summary>
    /// Generates all Combinatorial chemical isotopes given a structure.
    /// </summary>
    // @cdk.module  formula
    // @author      Miguel Rojas Cherto
    // @cdk.created 2007-11-20
    // @cdk.githash
    // @cdk.keyword isotope pattern
    public class IsotopePatternGenerator
    {
        private IChemObjectBuilder builder = null;
        private IsotopeFactory isoFactory;
        private IsotopePattern abundance_Mass = null;

       /// <summary> Minimal abundance of the isotopes to be added in the combinatorial search.</summary>
        private double minAbundance = .1;

        /// <summary>
        /// Constructor for the IsotopeGenerator. The minimum abundance is set to 
        ///                         0.1 (10% abundance) by default. 
        /// </summary>
        public IsotopePatternGenerator()
        : this(0.1)
        { }

        /// <summary>
        /// Constructor for the IsotopeGenerator.
        /// </summary>
        /// <param name="minAb">Minimal abundance of the isotopes to be added in the combinatorial search (scale 0.0 to 1.0)</param>
        public IsotopePatternGenerator(double minAb)
        {
            minAbundance = minAb;
            Trace.TraceInformation("Generating all Isotope structures with IsotopeGenerator");
        }

        /// <summary>
        /// Get all combinatorial chemical isotopes given a structure.
        /// </summary>
        /// <param name="molFor">The IMolecularFormula to start</param>
        /// <returns>A IsotopePattern object containing the different combinations</returns>
        public IsotopePattern GetIsotopes(IMolecularFormula molFor)
        {
            if (builder == null)
            {
                try
                {
                    isoFactory = Isotopes.Instance;
                    builder = molFor.Builder;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
            }
            string mf = MolecularFormulaManipulator.GetString(molFor, true);

            IMolecularFormula molecularFormula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula(mf, builder);

            foreach (var isos in molecularFormula.Isotopes)
            {
                string elementSymbol = isos.Symbol;
                int atomCount = molecularFormula.GetCount(isos);

                for (int i = 0; i < atomCount; i++)
                {
                    if (!CalculateAbundanceAndMass(elementSymbol))
                    {
                    }
                }
            }

            IsotopePattern isoP = IsotopePatternManipulator.SortAndNormalizedByIntensity(abundance_Mass);
            isoP = CleanAbundance(isoP, minAbundance);
            IsotopePattern isoPattern = IsotopePatternManipulator.SortByMass(isoP);
            return isoPattern;
        }

        /// <summary>
        /// Calculates the mass and abundance of all isotopes generated by adding one
        /// atom. Receives the periodic table element and calculate the isotopes, if
        /// there exist a previous calculation, add these new isotopes. In the
        /// process of adding the new isotopes, remove those that has an abundance
        /// less than setup parameter minAbundance, and remove duplicated masses.
        /// </summary>
        /// <param name="elementSymbol">The chemical element symbol</param>
        /// <returns>the calculation was successful</returns>
        private bool CalculateAbundanceAndMass(string elementSymbol)
        {
            var isotopes = isoFactory.GetIsotopes(elementSymbol);

            if (isotopes == null) return false;

            if (!isotopes.Any()) return false;

            double mass, previousMass, abundance, totalAbundance, newAbundance;

            var isotopeMassAndAbundance = new Dictionary<double, double>();
            IsotopePattern currentISOPattern = new IsotopePattern();

            // Generate isotopes for the current atom (element)
            foreach (var isotope in isotopes)
            {
                mass = isotope.ExactMass.Value;
                abundance = isotope.NaturalAbundance.Value;
                currentISOPattern.Isotopes.Add(new IsotopeContainer(mass, abundance));
            }

            // Verify if there is a previous calculation. If it exists, add the new
            // isotopes
            if (abundance_Mass == null)
            {
                abundance_Mass = currentISOPattern;
                return true;
            }
            else
            {
                for (int i = 0; i < abundance_Mass.Isotopes.Count; i++)
                {
                    totalAbundance = abundance_Mass.Isotopes[i].Intensity;

                    if (totalAbundance == 0) continue;

                    for (int j = 0; j < currentISOPattern.Isotopes.Count; j++)
                    {
                        abundance = currentISOPattern.Isotopes[j].Intensity;
                        mass = abundance_Mass.Isotopes[i].Mass;

                        if (abundance == 0) continue;

                        newAbundance = totalAbundance * abundance * 0.01;
                        mass += currentISOPattern.Isotopes[j].Mass;

                        // Filter duplicated masses
                        previousMass = SearchMass(isotopeMassAndAbundance.Keys, mass);
                        if (isotopeMassAndAbundance.ContainsKey(previousMass))
                        {
                            newAbundance += isotopeMassAndAbundance[previousMass];
                            mass = previousMass;
                        }

                        // Filter isotopes too small
                        if (newAbundance > 1E-10)
                        {
                            isotopeMassAndAbundance[mass] = newAbundance;
                        }
                        previousMass = 0;
                    }
                }

                abundance_Mass = new IsotopePattern();
                foreach (var mmass in isotopeMassAndAbundance.Keys)
                {
                    abundance_Mass.Isotopes.Add(new IsotopeContainer(mmass, isotopeMassAndAbundance[mmass]));
                }
            }

            return true;
        }

        /// <summary>
        /// Search the key mass in this Set.
        /// </summary>
        /// <param name="keySet">The Set object</param>
        /// <param name="mass">The mass to look for</param>
        /// <returns>The key value</returns>
        private double SearchMass(ICollection<Double> keySet, double mass)
        {
            double TOLERANCE = 0.00005f;
            double diff;
            foreach (var key in keySet)
            {
                diff = Math.Abs(key - mass);
                if (diff < TOLERANCE) return key;
            }

            return 0.0d;
        }

        /// <summary>
        /// Normalize the intensity (relative abundance) of all isotopes in relation
        /// of the most abundant isotope.
        /// </summary>
        /// <param name="isopattern">The IsotopePattern object</param>
        /// <param name="minAbundance">The minimum abundance</param>
        /// <returns>The IsotopePattern cleaned</returns>
        private IsotopePattern CleanAbundance(IsotopePattern isopattern, double minAbundance)
        {
            double intensity, biggestIntensity = 0.0f;

            foreach (var sc in isopattern.Isotopes)
            {
                intensity = sc.Intensity;
                if (intensity > biggestIntensity) biggestIntensity = intensity;
            }

            foreach (var sc in isopattern.Isotopes)
            {
                intensity = sc.Intensity;
                intensity /= biggestIntensity;
                if (intensity < 0) intensity = 0;

                sc.Intensity = intensity;
            }

            IsotopePattern sortedIsoPattern = new IsotopePattern();
            sortedIsoPattern.SetMonoIsotope(new IsotopeContainer(isopattern.Isotopes[0].Mass, isopattern.Isotopes[0].Intensity));
            for (int i = 1; i < isopattern.Isotopes.Count; i++)
            {
                if (isopattern.Isotopes[i].Intensity >= (minAbundance))
                    sortedIsoPattern.Isotopes.Add(new IsotopeContainer(isopattern.Isotopes[i].Mass, isopattern.Isotopes[i].Intensity));
            }
            return sortedIsoPattern;
        }
    }
}