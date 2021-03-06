/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Graphs;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs XlogP tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class XLogPDescriptorTest : MolecularDescriptorTest<XLogPDescriptor>
    {
        public XLogPDescriptor CreateDescriptor(bool checkAromaticity) => new XLogPDescriptor(checkAromaticity);

        [TestMethod(), Ignore()]
        public void Testno688()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C(O)c1[nH0]cccc1"); // xlogp training set molecule no688
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-1.69, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno1596()
        {
            // the xlogp program value is 0.44 because of paralleled donor pair correction factor
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("Nc2ccc(S(=O)(=O)c1ccc(N)cc1)cc2"); // xlogp training set molecule no1596
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(0.86, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Testno367()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C(O)C(N)CCCN"); // xlogp training set molecule no367
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-3.30, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno1837()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=P(N1CC1)(N2CC2)N3CC3"); // xlogp training set molecule no1837
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-1.19, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno87()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("c1cc2ccc3ccc4ccc5cccc6c(c1)c2c3c4c56"); // xlogp training set molecule no87
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(7.00, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno1782()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("S1C2N(C(=O)C2NC(=O)C(c2ccccc2)C(=O)O)C(C(=O)O)C1(C)C"); // xlogp training set molecule no30
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(1.84, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno30()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(#Cc1ccccc1)c1ccccc1"); // xlogp training set molecule no30
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(4.62, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod(), Ignore()]
        public void Testno937()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("ClCC(O)C[nH0]1c([nH0]cc1[N+](=O)[O-])C"); // xlogp training set molecule no937
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(0.66, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno990()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("FC(F)(F)c1ccc(cc1)C(=O)N"); // xlogp training set molecule no990
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(1.834, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Testno1000()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("Clc1cccc(c1)/C=C/[N+](=O)[O-]"); // xlogp training set molecule no1000
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(2.809, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void TestApirinBug1296383()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(=O)OC1=CC=CC=C1C(=O)O"); // aspirin
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(1.422, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno1429()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C(OC)CNC(=O)c1ccc(N)cc1"); // xlogp training set molecule no1429
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(0.31, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Testno1274()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=[N+]([O-])c1ccc(cc1)CC(N)C(=O)O"); // xlogp training set molecule no1274
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-1.487, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Testno454()
        {
            //xlogp program gives a result of -0.89, because one N is classified as in ring and not as amid
            //if one takes a 5 or 7 ring than the program assignes amid ... strange
            //sometimes amid is O=C-N-C=O sometimes not...
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C1NC(=O)C=CN1C1OC(CO)C(O)C1O"); // xlogp training set molecule no454
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-2.11, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void Testno498()
        {
            //even here the amid assignment is very strange
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C1N(C)C=CC(=O)N1C"); // xlogp training set molecule no498
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(-0.59, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void TestAprindine()
        {
            //even here the amid assignment is very strange
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCN(CC)CCCN(C2Cc1ccccc1C2)c3ccccc3"); // xlogp training set molecule Aprindine
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(5.03, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Test1844()
        {
            var sp = CDK.SmilesParser;
            // SMILES is in octet-rule version, PubChem has normalized one
            var mol = sp.ParseSmiles("Brc1cc(Cl)c(O[P+]([S-])(OC)OC)cc1Cl"); // xlogp training set molecule 1844
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(5.22, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        [TestMethod()]
        public void Test1810()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("Clc1ccc2Sc3ccccc3N(CCCN3CCN(C)CC3)c2c1"); // xlogp training set molecule 1810
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(4.56, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 1.0); //at:  16
        }

        /// <summary>
        // @cdk.inchi InChI=1/C23H20N2O3S/c26-22-21(16-17-29(28)20-14-8-3-9-15-20)23(27)25(19-12-6-2-7-13-19)24(22)18-10-4-1-5-11-18/h1-15,21H,16-17H2
        /// </summary>
        [TestMethod()]
        public void Test1822()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[S+]([O-])(CCC1C(=O)N(N(c2ccccc2)C1=O)c1ccccc1)c1ccccc1"); // xlogp training set molecule 1822
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(2.36, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: false).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void TestAromaticBenzene()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C1=CC=CC=C1"); // benzene
            Aromaticity aromaticity = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllSimpleFinder);
            aromaticity.Apply(mol);
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(2.02, CreateDescriptor(false).Calculate(mol, correctSalicylFactor: true).Value, 0.01);
        }

        [TestMethod()]
        public void TestNonAromaticBenzene()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C1=CC=CC=C1"); // benzene
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(2.08, CreateDescriptor(false).Calculate(mol, correctSalicylFactor: true).Value, 0.01);
        }

        [TestMethod()]
        public void TestPerceivedAromaticBenzene()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C1=CC=CC=C1"); // benzene
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            Assert.AreEqual(2.02, CreateDescriptor(true).Calculate(mol, correctSalicylFactor: true).Value, 0.01);
        }
    }
}
