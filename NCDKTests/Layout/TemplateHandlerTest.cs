/* Copyright (C) 2003-2007  Christoph Steinbeck
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Templates;
using System.Diagnostics;
using System.IO;

namespace NCDK.Layout
{
    // @cdk.module  test-sdg
    // @author      steinbeck
    // @cdk.created September 4, 2003
    [TestClass()]
    public class TemplateHandlerTest : CDKTestCase
    {
        public bool standAlone = false;

        private static SmilesParser sp = null;
        private static StructureDiagramGenerator sdg = null;

        static TemplateHandlerTest()
        {
            sdg = new StructureDiagramGenerator();
            sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        }

        [TestMethod()]
        public void TestInit()
        {
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);

            Assert.AreEqual(5, th.TemplateCount);
        }

        [TestMethod()]
        public void TestDetection()
        {
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            string smiles = "CC12C3(C6CC6)C4(C)C1C5(C(CC)C)C(C(CC)C)2C(C)3C45CC(C)C";
            IAtomContainer mol = sp.ParseSmiles(smiles);
            Assert.IsTrue(th.MapTemplates(mol));
        }

        /// <summary>
        /// Tests if a template matches if just an element is non-carbon.
        /// </summary>
        [TestMethod()]
        public void TestOtherElements()
        {
            bool itIsInThere = false;
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = TestMoleculeFactory.MakeSteran();
            itIsInThere = th.MapTemplates(mol);
            Assert.IsTrue(itIsInThere);
            mol.Atoms[0].Symbol = "N";
            itIsInThere = th.MapTemplates(mol);
            Assert.IsTrue(itIsInThere);
        }

        /// <summary>
        /// Tests if a template matches if just and bond order is changed.
        /// </summary>
        [TestMethod()]
        public void TestOtherBondOrder()
        {
            bool itIsInThere = false;
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = TestMoleculeFactory.MakeSteran();
            itIsInThere = th.MapTemplates(mol);
            Assert.IsTrue(itIsInThere);
            mol.Bonds[0].Order = BondOrder.Double;
            itIsInThere = th.MapTemplates(mol);
            Assert.IsTrue(itIsInThere);
        }

        [TestMethod()]
        public void TestAddMolecule()
        {
            Debug.WriteLine("***TestAddMolecule***");
            bool itIsInThere = false;
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            sdg.Molecule = mol;
            sdg.GenerateCoordinates();
            mol = sdg.Molecule;

            string smiles = "C1=C(C)C2CC(C1)C2(C)(C)";
            IAtomContainer smilesMol = sp.ParseSmiles(smiles);
            itIsInThere = th.MapTemplates(smilesMol);
            Debug.WriteLine("Alpha-Pinene found by templateMapper: " + itIsInThere);
            Assert.IsFalse(itIsInThere);
            th.AddMolecule(mol);
            Debug.WriteLine("now adding template for alpha-Pinen and trying again.");
            itIsInThere = th.MapTemplates(smilesMol);
            Debug.WriteLine("Alpha-Pinene found by templateMapper: " + itIsInThere);
            Assert.IsTrue(itIsInThere);
        }

        [TestMethod()]
        public void TestRemoveMolecule()
        {
            Debug.WriteLine("***TestRemoveMolecule***");
            bool itIsInThere = false;
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            sdg.Molecule = mol;
            sdg.GenerateCoordinates();
            mol = sdg.Molecule;

            string smiles = "C1=C(C)C2CC(C1)C2(C)(C)";
            IAtomContainer smilesMol = sp.ParseSmiles(smiles);
            itIsInThere = th.MapTemplates(smilesMol);
            Debug.WriteLine("Alpha-Pinene found by templateMapper: " + itIsInThere);
            Assert.IsFalse(itIsInThere);
            th.AddMolecule(mol);
            Debug.WriteLine("now adding template for alpha-Pinen and trying again.");
            itIsInThere = th.MapTemplates(smilesMol);
            Debug.WriteLine("Alpha-Pinene found by templateMapper: " + itIsInThere);
            Assert.IsTrue(itIsInThere);
            Debug.WriteLine("now removing template for alpha-Pinen again and trying again.");
            th.RemoveMolecule(mol);
            itIsInThere = th.MapTemplates(smilesMol);
            Debug.WriteLine("Alpha-Pinene found by templateMapper: " + itIsInThere);
            Assert.IsFalse(itIsInThere);

        }

        /// <summary>
        /// Loads a molecule with two adamantanes and one cubane
        /// substructure and tests whether all are found.
        /// </summary>
        public void GetMappedSubstructures_IAtomContainer()
        {
            // Set up molecule reader
            string filename = "NCDK.Data.MDL.diadamantane-cubane.mol";
            Stream ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader molReader = new MDLReader(ins, ChemObjectReaderModes.Strict);

            // Read molecule
            IAtomContainer molecule = (IAtomContainer)molReader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());

            // Map templates
            TemplateHandler th = new TemplateHandler(Default.ChemObjectBuilder.Instance);
            var mappedStructures = th.GetMappedSubstructures(molecule);

            // Do the Assert.assertion
            Assert.AreEqual(3, mappedStructures.Count, "3 mapped templates");
        }

        [TestMethod()]
        public void Convert()
        {
            TemplateHandler templateHandler = new TemplateHandler(Silent.ChemObjectBuilder.Instance);
            using (var bout = new MemoryStream())
            {
                templateHandler.ToIdentityTemplateLibrary().Store(bout);
                Assert.AreEqual(
                    "C1C2CC3CC1CC(C2)C3 |(-1.07,-1.59,;.38,-1.21,;1.82,-1.59,;1.07,-.29,;-.38,-.67,;-1.82,-.29,;-1.82,1.21,;-.37,1.59,;.38,.29,;1.07,1.21,)|\n"
                    + "C12C3C4C1C5C2C3C45 |(.62,-.99,;-.88,-1.12,;-1.23,.43,;.26,.57,;.88,1.12,;1.23,-.43,;-.26,-.57,;-.62,.98,)|\n"
                    + "C1C2CC3C4CC5CC(C14)C(C2)C3C5 |(-1.82,2.15,;-.37,2.53,;1.07,2.15,;1.07,.65,;-.38,.27,;-.38,-1.23,;.37,-2.53,;-1.07,-2.15,;-1.07,-.65,;-1.82,.65,;.38,-.27,;.38,1.23,;1.82,-.65,;1.82,-2.15,)|\n"
                    + "C1CCC2C(C1)CCC3C4CCCC4CCC23 |(-6.51,.72,;-6.51,-.78,;-5.22,-1.53,;-3.92,-.78,;-3.92,.72,;-5.22,1.47,;-2.62,1.47,;-1.32,.72,;-1.32,-.78,;-.02,-1.53,;1.41,-1.07,;2.29,-2.28,;1.41,-3.49,;-.02,-3.03,;-1.32,-3.78,;-2.62,-3.03,;-2.62,-1.53,)|\n"
                    + "C1CCCCCCCCCCCCC1 |(-.04,1.51,;1.26,.76,;1.26,-.74,;2.56,-1.49,;2.56,-2.99,;1.29,-3.72,;1.31,-5.29,;-.09,-5.89,;-1.34,-5.24,;-1.34,-3.74,;-2.63,-2.99,;-2.63,-1.49,;-1.34,-.74,;-1.34,.76,)|\n",
                    System.Text.Encoding.UTF8.GetString(bout.ToArray()));
            }
        }
    }
}
