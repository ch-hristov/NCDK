/* Copyright (C) 2007-2011  Egon Willighagen <egonw@users.sf.net>
 *               2007       Rajarshi Guha
 *                    2011  Nimish Gopal <nimishg@ebi.ac.uk>
 *                    2011  Syed Asad Rahman <asad@ebi.ac.uk>
 *                    2011  Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Default;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// CDK atom type list. All tests in this class <b>must</b> use
    /// explicit <see cref="IAtomContainer"/>s; test using data files
    /// must be placed in <see cref="CDKAtomTypeMatcherFilesTest"/>.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class CDKAtomTypeMatcherTest : AbstractCDKAtomTypeTest
    {
        private static IDictionary<string, int> testedAtomTypes = new Dictionary<string, int>();

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder_int()
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance,
                    CDKAtomTypeMatcher.RequireExplicitHydrogens);
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer_IAtom()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            Hybridization thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            Hybridization thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            // just check consistency; other methods do perception testing
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance);
            IAtomType[] types = matcher.FindMatchingAtomTypes(mol);
            for (int i = 0; i < types.Length; i++)
            {
                IAtomType type = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                Assert.AreEqual(type.AtomTypeName, types[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestDummy()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new PseudoAtom("R");
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "X" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 2445178
        [TestMethod()]
        public void TestNonExistingType()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("Error");
            mol.Atoms.Add(atom);
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance);
            IAtomType type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            Assert.AreEqual("X", type.AtomTypeName);
        }

        [TestMethod()]
        public void TestEthene()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthyneKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            atom2.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.sp", "C.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthyneRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.sp", "C.radical.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestImine()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "N.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestImineRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "N.sp2.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEtheneRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.radical.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestGuanineMethyl()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("N");
            IAtom atom5 = new Atom("C");
            IAtom atom6 = new Atom("C");
            IAtom atom7 = new Atom("N");
            IAtom atom8 = new Atom("C");
            IAtom atom9 = new Atom("C");
            IAtom atom10 = new Atom("N");
            IAtom atom11 = new Atom("O");
            IAtom atom12 = new Atom("N");
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.Atoms.Add(atom8);
            mol.Atoms.Add(atom9);
            mol.Atoms.Add(atom10);
            mol.Atoms.Add(atom11);
            mol.Atoms.Add(atom12);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[8], BondOrder.Double);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double);
            mol.AddBond(mol.Atoms[7], mol.Atoms[9], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[10], BondOrder.Double);
            mol.AddBond(mol.Atoms[8], mol.Atoms[11], BondOrder.Single);
            mol.AddBond(mol.Atoms[8], mol.Atoms[9], BondOrder.Single);

            string[] expectedTypes = {"C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp3", "C.sp2", "N.sp2", "C.sp2", "C.sp2",
                "N.amide", "O.sp2", "N.sp3"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPropyne()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "C.sp", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFormaldehyde()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "O.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarboxylate()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("O");
            atom2.FormalCharge = -1;
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2.co2", "C.sp2", "O.minus.co2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFormaldehydeRadicalKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            atom.FormalCharge = +1;
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "O.plus.sp2.radical", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Aim of this test is to see if the atom type matcher is OK with
        /// partial filled implicit hydrogen counts.
        /// </summary>
        [TestMethod()]
        public void TestPartialMethane()
        {
            IAtomContainer methane = new AtomContainer();
            IAtom carbon = new Atom("C");
            methane.Atoms.Add(carbon);

            string[] expectedTypes = { "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 2;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 3;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 4;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);
        }

        [TestMethod()]
        public void TestMethanol()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestLithiumMethanoxide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("Li");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.sp3", "C.sp3", "Li" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHCN()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "N.sp1", "C.sp" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHNO2()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("O");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("H");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp2", "O.sp2", "O.minus", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNitromethane()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("O");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "N.nitro", "O.sp2", "O.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylAmine()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylAmineRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            atom.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp3.radical", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneImine()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "N.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthene_withHybridInfo()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            Hybridization thisHybridization = Hybridization.SP2;
            atom.Hybridization = thisHybridization;
            atom2.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPiperidine()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakePiperidine();
            string[] expectedTypes = { "N.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTetrahydropyran()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeTetrahydropyran();
            string[] expectedTypes = { "O.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestS3()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, o1, BondOrder.Double);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(s, o2, BondOrder.Double);

            mol.Atoms.Add(s);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.oxide", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestH2S()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");
            IAtom h2 = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, h1, BondOrder.Single);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(s, h2, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom se = Default.ChemObjectBuilder.Instance.CreateAtom("Se");
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");
            IAtom h2 = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(se, h1, BondOrder.Single);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(se, h2, BondOrder.Single);

            mol.Atoms.Add(se);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Se.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se_oneImplH()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom se = Default.ChemObjectBuilder.Instance.CreateAtom("Se");
            se.ImplicitHydrogenCount = 1;
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(se, h1, BondOrder.Single);

            mol.Atoms.Add(se);
            mol.Atoms.Add(h1);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Se.3", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se_twoImplH()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom se = Default.ChemObjectBuilder.Instance.CreateAtom("Se");
            se.ImplicitHydrogenCount = 2;
            mol.Atoms.Add(se);

            string[] expectedTypes = { "Se.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSelenide()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom se = Default.ChemObjectBuilder.Instance.CreateAtom("Se");
            se.ImplicitHydrogenCount = 0;
            se.FormalCharge = -2;
            mol.Atoms.Add(se);

            string[] expectedTypes = { "Se.2minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestH2S_Hybridization()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            s.Hybridization = Hybridization.SP3;
            mol.Atoms.Add(s);
            string[] expectedTypes = { "S.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHS()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            s.FormalCharge = -1;
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, h1, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "S.minus", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSOCharged()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            atom.FormalCharge = -1;
            IAtom atom2 = new Atom("S");
            atom2.FormalCharge = 1;
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "S.inyl.charged", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSO()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "S.inyl", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSOO()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("O");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "O.sp2", "S.onyl", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestStrioxide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("O");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = { "O.sp2", "O.sp2", "S.trioxide", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmineOxide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "N.oxide", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestThioAmide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("S");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "S.2", "C.sp2", "N.thioamide" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAdenine()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAdenine();
            string[] expectedTypes = {"C.sp2", "C.sp2", "C.sp2", "N.sp2", "N.sp2", "N.planar3", "N.sp2", "N.sp3", "C.sp2",
                "C.sp2"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide2()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide3()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestLactam()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestThioAcetone()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("S");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "S.2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSulphuricAcid()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "S.onyl", "O.sp2", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/CH4O2S2/c1-5(2,3)4/h1H3,(H,2,3,4)
        [TestMethod()]
        public void TestThioSulphonate()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("S");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "S.thionyl", "S.2", "O.sp3", "O.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSulphuricAcid_Charged()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +2;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "S.onyl.charged", "O.minus", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSF6()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("S");
            IAtom atom3 = new Atom("F");
            IAtom atom4 = new Atom("F");
            IAtom atom5 = new Atom("F");
            IAtom atom6 = new Atom("F");
            IAtom atom7 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "F", "S.octahedral", "F", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMnF4()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("Mn");
            IAtom atom3 = new Atom("F");
            IAtom atom4 = new Atom("F");
            IAtom atom5 = new Atom("F");
            mol.Atoms.Add(atom);
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +2;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            atom4.FormalCharge = -1;
            mol.Atoms.Add(atom5);
            atom5.FormalCharge = -1;

            string[] expectedTypes = { "F.minus", "Mn.2plus", "F.minus", "F.minus", "F.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCrF6()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("Cr");
            IAtom atom3 = new Atom("F");
            IAtom atom4 = new Atom("F");
            IAtom atom5 = new Atom("F");
            IAtom atom6 = new Atom("F");
            IAtom atom7 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "F", "Cr", "F", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestXeF4()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("Xe");
            IAtom atom3 = new Atom("F");
            IAtom atom4 = new Atom("F");
            IAtom atom5 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "F", "Xe.3", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphate()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("P");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "P.ate", "O.sp3", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C3H10OP/c1-5(2,3)4/h4H,1-3H3/q+1
        [TestMethod()]
        public void TestHydroxyTriMethylPhophanium()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("P");
            atom2.FormalCharge = +1;
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "P.ate.charged", "C.sp3", "C.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphateCharged()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            atom.FormalCharge = -1;
            IAtom atom2 = new Atom("P");
            atom2.FormalCharge = 1;
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "P.ate.charged", "O.sp3", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphorusTriradical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("P");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "P.se.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmmonia()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("H");
            IAtom atom4 = new Atom("H");
            IAtom atom5 = new Atom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "H", "N.plus", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNitrogenRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "H", "N.sp3.radical", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTMS()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Si");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Si.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTinCompound()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Sn");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Sn.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestArsenicPlus()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("As");
            atom2.FormalCharge = +1;
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            IAtom atom5 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "As.plus", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphine()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom2 = new Atom("P");
            IAtom atom3 = new Atom("H");
            IAtom atom4 = new Atom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "P.ine", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/HO3P/c1-4(2)3/h(H-,1,2,3)/p+1
        [TestMethod()]
        public void TestPhosphorousAcid()
        {
            IAtomContainer mol = new AtomContainer();
            IChemObjectBuilder builder = mol.Builder;
            IAtom a1 = builder.CreateAtom("P");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("O");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("H");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("H");
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "P.anium", "O.sp3", "O.sp3", "O.sp2", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDiethylPhosphine()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("P");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "P.ine", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C2H5P/c1-3-2/h1H2,2H3
        [TestMethod()]
        public void TestPhosphorCompound()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("P");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "P.irane", "C.sp3" }; // FIXME: compare with previous test... can't both be P.ine...
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbokation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom2 = new Atom("C");
            atom2.FormalCharge = +1;
            IAtom atom3 = new Atom("H");
            IAtom atom4 = new Atom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "C.plus.planar", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbokation_implicitHydrogen()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom2 = new Atom("C");
            atom2.FormalCharge = +1;
            mol.Atoms.Add(atom2);

            string[] expectedTypes = { "C.plus.sp2" }; // FIXME: compare with previous test... same compound!
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydrogen()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxyl()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom oxygen = new Atom("O");
            oxygen.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "H", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxyl2()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom oxygen = new Atom("O");
            oxygen.FormalCharge = -1;
            mol.Atoms.Add(oxygen);

            string[] expectedTypes = { "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxonium()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom1 = new Atom("H");
            IAtom atom2 = new Atom("H");
            IAtom oxygen = new Atom("O");
            oxygen.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "H", "H", "O.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPositiveCarbonyl()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            IAtom atom1 = new Atom("H");
            IAtom atom2 = new Atom("H");
            IAtom oxygen = new Atom("O");
            IAtom carbon = new Atom("C");
            oxygen.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(oxygen);
            mol.Atoms.Add(carbon);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);

            string[] expectedTypes = { "H", "H", "H", "O.plus.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestProton()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            atom.FormalCharge = 1;
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "H.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHalides()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("Cl");
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            string[] expectedTypes = { "Cl.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("F");
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "F.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Br");
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Br.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("I");
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "I.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHalogens()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("Cl");
            IAtom hydrogen = new Atom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            string[] expectedTypes = new string[] { "Cl", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "I", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "Br", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "F", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFluorRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "F.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChlorRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("Cl");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "Cl.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBromRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("Br");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "Br.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestIodRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("I");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "I.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestIMinusF2()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("I");
            IAtom atom3 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            atom2.FormalCharge = -1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "F", "I.minus.5", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydride()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);

            string[] expectedTypes = new string[] { "H.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydrogenRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("H");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = new string[] { "H.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("N");
            atom2.FormalCharge = -1;
            IAtom atom3 = new Atom("N");
            atom3.FormalCharge = +1;
            IAtom atom4 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Triple);

            string[] expectedTypes = new string[] { "C.sp3", "N.minus.sp3", "N.plus.sp1", "N.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAllene()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);

            string[] expectedTypes = new string[] { "C.sp2", "C.allene", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzide2()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("N");
            IAtom atom3 = new Atom("N");
            atom3.FormalCharge = +1;
            IAtom atom4 = new Atom("N");
            atom4.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = new string[] { "C.sp3", "N.sp2", "N.plus.sp1", "N.minus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMercuryComplex()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("Hg");
            atom.FormalCharge = -1;
            IAtom atom1 = new Atom("O");
            atom1.FormalCharge = +1;
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[4], mol.Atoms[0], BondOrder.Single);
            string[] expectedTypes = new string[] { "Hg.minus", "O.plus.sp2", "C.sp2", "C.sp2", "N.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Hg");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Hg.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Hg");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Hg.plus", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Hg.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Hg.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Hg.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPoloniumComplex()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("O");
            IAtom atom1 = new Atom("Po");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            string[] expectedTypes = new string[] { "O.sp3", "Po", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestStronglyBoundKations()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms[1].FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IAtom atom = new Atom("Na");
            mol.Atoms.Add(atom);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = new string[] { "C.sp2", "O.plus.sp2", "Na" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMetallics()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("W");
            mol.Atoms.Add(atom);
            string[] expectedTypes = new string[] { "W.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("K");
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "K.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Co");
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSalts()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom = new Atom("Na");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            string[] expectedTypes = new string[] { "Na.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("K");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "K.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Ca");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Ca.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Mg");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Mg.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Ni");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Ni.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Pt");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Pt.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Co");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Co");
            atom.FormalCharge = +3;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Cu");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Cu.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = new AtomContainer();
            atom = new Atom("Al");
            atom.FormalCharge = +3;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Al.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Fix_Ca_2()
        {
            //string molName = "Ca_2";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ca");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Ca.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Fix_Ca_1()
        {
            //string molName1 = "Ca_1";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ca");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes1 = { "Ca.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void TestCyclopentadienyl()
        {
            IAtomContainer cp = new AtomContainer();
            cp.Atoms.Add(new Atom("C"));
            cp.Atoms[0].Hybridization = Hybridization.SP2;
            cp.Atoms[0].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(new Atom("C"));
            cp.Atoms[1].Hybridization = Hybridization.SP2;
            cp.Atoms[1].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(new Atom("C"));
            cp.Atoms[2].Hybridization = Hybridization.SP2;
            cp.Atoms[2].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(new Atom("C"));
            cp.Atoms[3].Hybridization = Hybridization.SP2;
            cp.Atoms[3].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(new Atom("C"));
            cp.Atoms[4].FormalCharge = -1;
            cp.Atoms[4].Hybridization = Hybridization.Planar3;
            cp.Atoms.Add(new Atom("H"));
            cp.AddBond(cp.Atoms[0], cp.Atoms[1], BondOrder.Double);
            cp.AddBond(cp.Atoms[1], cp.Atoms[2], BondOrder.Single);
            cp.AddBond(cp.Atoms[2], cp.Atoms[3], BondOrder.Double);
            cp.AddBond(cp.Atoms[3], cp.Atoms[4], BondOrder.Single);
            cp.AddBond(cp.Atoms[4], cp.Atoms[0], BondOrder.Single);
            cp.AddBond(cp.Atoms[4], cp.Atoms[5], BondOrder.Single);

            string[] expectedTypes = new string[] { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.minus.planar", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, cp);
        }

        [TestMethod()]
        public void TestFerrocene()
        {
            IAtomContainer ferrocene = new AtomContainer();
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms[4].FormalCharge = -1;
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms.Add(new Atom("C"));
            ferrocene.Atoms[9].FormalCharge = -1;
            ferrocene.Atoms.Add(new Atom("Fe"));
            ferrocene.Atoms[10].FormalCharge = +2;
            ferrocene.AddBond(ferrocene.Atoms[0], ferrocene.Atoms[1], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[1], ferrocene.Atoms[2], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[2], ferrocene.Atoms[3], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[3], ferrocene.Atoms[4], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[4], ferrocene.Atoms[0], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[5], ferrocene.Atoms[6], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[6], ferrocene.Atoms[7], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[7], ferrocene.Atoms[8], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[8], ferrocene.Atoms[9], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[9], ferrocene.Atoms[5], BondOrder.Single);

            string[] expectedTypes = new string[]{"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.minus.planar", "C.sp2", "C.sp2",
                "C.sp2", "C.sp2", "C.minus.planar", "Fe.2plus"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, ferrocene);
        }

        [TestMethod()]
        public void TestFuran()
        {
            IAtomContainer furan = new AtomContainer();
            furan.Atoms.Add(new Atom("C"));
            furan.Atoms.Add(new Atom("C"));
            furan.Atoms.Add(new Atom("C"));
            furan.Atoms.Add(new Atom("C"));
            furan.Atoms.Add(new Atom("O"));
            furan.AddBond(furan.Atoms[0], furan.Atoms[1], BondOrder.Double);
            furan.AddBond(furan.Atoms[1], furan.Atoms[2], BondOrder.Single);
            furan.AddBond(furan.Atoms[2], furan.Atoms[3], BondOrder.Double);
            furan.AddBond(furan.Atoms[3], furan.Atoms[4], BondOrder.Single);
            furan.AddBond(furan.Atoms[4], furan.Atoms[0], BondOrder.Single);
            string[] expectedTypes = new string[] { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.planar3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, furan);
        }

        [TestMethod()]
        public void TestPerchlorate()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("Cl");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.perchlorate", "O.sp2", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

         /// <summary>
         /// Gallium tetrahydroxide.
         /// </summary>
        [TestMethod()]
        public void TestGallate()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            atom.FormalCharge = -1;
            IAtom atom2 = new Atom("Ga");
            atom2.FormalCharge = +3;
            IAtom atom3 = new Atom("O");
            atom3.FormalCharge = -1;
            IAtom atom4 = new Atom("O");
            atom4.FormalCharge = -1;
            IAtom atom5 = new Atom("O");
            atom5.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);

            string[] expectedTypes = new string[] { "O.minus", "Ga.3plus", "O.minus", "O.minus", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

         /// <summary>
         /// Gallium trihydroxide.
         /// </summary>
        [TestMethod()]
        public void TestGallateCovalent()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("Ga");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);

            string[] expectedTypes = new string[] { "O.sp3", "Ga", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPerchlorate_ChargedBonds()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("Cl");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            IAtom atom5 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +3;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            atom4.FormalCharge = -1;
            mol.Atoms.Add(atom5);
            atom5.FormalCharge = -1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.perchlorate.charged", "O.minus", "O.minus", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChlorate()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("Cl");
            IAtom atom3 = new Atom("O");
            IAtom atom4 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.chlorate", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestOxide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            atom.FormalCharge = -2;
            mol.Atoms.Add(atom);

            string[] expectedTypes = new string[] { "O.minus2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzulene()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAzulene();
            string[] expectedTypes = new string[]{"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2",
                "C.sp2", "C.sp2"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestIndole()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "N.planar3" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIndole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

         /// <summary>
         /// Test for the structure in XLogPDescriptorTest.Testno937().
         /// </summary>
        [TestMethod()]
        public void Testno937()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp2", "C.sp3" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            molecule.Atoms[3].Symbol = "N";
            molecule.Atoms.Add(molecule.Builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestBenzene()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = new AtomContainer();
            molecule.Add(new Ring(6, "C"));
            foreach (var bond in molecule.Bonds)
            {
                bond.IsAromatic = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestBenzene_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = new AtomContainer();
            molecule.Add(new Ring(6, "C"));
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrrole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrrole_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrroleAnion()
        {
            string[] expectedTypes = { "C.sp2", "N.minus.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrroleAnion();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestImidazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeImidazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void Test124Triazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "C.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.Make124Triazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void Test123Triazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.Make123Triazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTetrazole()
        {
            string[] expectedTypes = { "N.sp2", "N.planar3", "N.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeTetrazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestOxazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "C.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeOxazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestIsoxazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIsoxazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        // testThiazole can be found below...

        [TestMethod()]
        public void TestIsothiazole()
        {
            string[] expectedTypes = { "C.sp2", "S.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIsothiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestThiadiazole()
        {
            string[] expectedTypes = { "C.sp2", "S.planar3", "C.sp2", "N.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeThiadiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestOxadiazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "C.sp2", "N.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeOxadiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridine_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineDirect()
        {
            string[] expectedTypes = { "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("N"));
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Double);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1957958
        [TestMethod()]
        public void TestPyridineWithSP2()
        {
            string[] expectedTypes = { "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("N");
            IAtom a2 = mol.Builder.CreateAtom("C");
            IAtom a3 = mol.Builder.CreateAtom("C");
            IAtom a4 = mol.Builder.CreateAtom("C");
            IAtom a5 = mol.Builder.CreateAtom("C");
            IAtom a6 = mol.Builder.CreateAtom("C");

            a1.Hybridization = Hybridization.SP2;
            a2.Hybridization = Hybridization.SP2;
            a3.Hybridization = Hybridization.SP2;
            a4.Hybridization = Hybridization.SP2;
            a5.Hybridization = Hybridization.SP2;
            a6.Hybridization = Hybridization.SP2;

            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Atoms.Add(a5);
            mol.Atoms.Add(a6);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1879589
        [TestMethod()]
        public void TestChargedSulphurSpecies()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "S.plus", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            molecule.Atoms[4].Symbol = "S";
            molecule.Atoms[4].FormalCharge = +1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineOxide_Charged()
        {
            string[] expectedTypes = { "C.sp2", "N.plus.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.minus" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridineOxide();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineOxide()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("N")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("O")); // 6

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Double); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Double); // 7

            string[] expectedTypes = { "C.sp2", "N.sp2.3", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPyridineOxide_SP2()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms[0].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("N")); // 1
            mol.Atoms[1].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms[2].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms[3].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms[4].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms[5].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("O")); // 6
            mol.Atoms[6].Hybridization = Hybridization.SP2;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Double); // 7

            string[] expectedTypes = { "C.sp2", "N.sp2.3", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPyridineOxideCharged_SP2()
        {
            string[] expectedTypes = { "C.sp2", "N.plus.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.minus" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridineOxide();
            foreach (var bond in molecule.Bonds)
                bond.Order = BondOrder.Single;
            for (int i = 0; i < 6; i++)
            {
                molecule.Atoms[i].Hybridization = Hybridization.SP2;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrimidine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrimidine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridazine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridazine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTriazine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "N.sp2", "C.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeTriazine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestThiazole()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "S.planar3", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeThiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        /// <summary>
        /// SDF version of the PubChem entry for the given InChI uses uncharged Ni.
        /// </summary>
        // @cdk.inchi InChI=1/C2H6S2.Ni/c3-1-2-4;/h3-4H,1-2H2;/q;+2/p-2/fC2H4S2.Ni/h3-4h;/q-2;m
        [TestMethod()]
        public void TestNiCovalentlyBound()
        {
            string[] expectedTypes = { "C.sp3", "C.sp3", "S.3", "Ni", "S.3" };
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("S"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("Ni"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("S"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsF()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom carbon1 = new Atom("C");
            IAtom carbon2 = new Atom("C");

            IAtom atom = new Atom("F");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "F.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsCl()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom carbon1 = new Atom("C");
            IAtom carbon2 = new Atom("C");

            IAtom atom = new Atom("Cl");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Cl.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsBr()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom carbon1 = new Atom("C");
            IAtom carbon2 = new Atom("C");

            IAtom atom = new Atom("Br");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Br.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsI()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom carbon1 = new Atom("C");
            IAtom carbon2 = new Atom("C");

            IAtom atom = new Atom("I");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "I.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestRearrangementCarbokation()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom carbon1 = new Atom("C");
            carbon1.FormalCharge = +1;
            IAtom carbon2 = new Atom("C");
            IAtom carbon3 = new Atom("C");

            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Atoms.Add(carbon3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.plus.sp2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChargedSpecies()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom1 = new Atom("C");
            atom1.FormalCharge = -1;
            IAtom atom2 = new Atom("O");
            atom2.FormalCharge = +1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.minus.sp1", "O.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        //    [O+]=C-[C-]
        [TestMethod()]
        public void TestChargedSpecies2()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom1 = new Atom("O");
            atom1.FormalCharge = 1;
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            atom3.FormalCharge = -1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.plus.sp2", "C.sp2", "C.minus.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        //    [C-]=C-C
        [TestMethod()]
        public void TestChargedSpecies3()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom1 = new Atom("C");
            atom1.FormalCharge = -1;
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.minus.sp2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // [C-]#[N+]C
        [TestMethod()]
        public void TestIsonitrile()
        {
            IAtomContainer mol = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("N");
            atom2.FormalCharge = 1;
            IAtom atom3 = new Atom("C");
            atom3.FormalCharge = -1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);

            string[] expectedTypes = { "C.sp3", "N.plus.sp1", "C.minus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNobleGases()
        {
            IAtomContainer mol = new AtomContainer();

            mol.Atoms.Add(new Atom("He"));
            mol.Atoms.Add(new Atom("Ne"));
            mol.Atoms.Add(new Atom("Ar"));
            mol.Atoms.Add(new Atom("Kr"));
            mol.Atoms.Add(new Atom("Xe"));
            mol.Atoms.Add(new Atom("Rn"));

            string[] expectedTypes = { "He", "Ne", "Ar", "Kr", "Xe", "Rn" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestZincChloride()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Zn"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Zn", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestZinc()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Zn"));
            mol.Atoms[0].FormalCharge = +2;

            string[] expectedTypes = { "Zn.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSilicon()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("Si");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a17);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a5, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a5, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a6, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a6, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a6, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a7, a14, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a7, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.CreateBond(a7, a16, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.CreateBond(a1, a17, BondOrder.Single);
            mol.Bonds.Add(b16);

            string[] expectedTypes = {"Si.sp3", "O.sp3", "O.sp3", "O.sp3", "C.sp3", "C.sp3", "C.sp3", "H", "H", "H", "H",
                "H", "H", "H", "H", "H", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestScandium()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Sc"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Single);

            string[] expectedTypes = {"Sc.3minus", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H",
                "O.sp3", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestVanadium()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("V"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Triple);

            string[] expectedTypes = {"V.3minus", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1",
                "C.sp", "N.sp1", "C.sp", "N.sp1"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTitanium()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Ti"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Triple);
            mol.Atoms.Add(new Atom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(new Atom("N"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Triple);

            string[] expectedTypes = {"Ti.3minus", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1",
                "C.sp", "N.sp1", "C.sp", "N.sp1"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBoronTetraFluoride()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("B"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "B.minus", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBerylliumTetraFluoride()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Be"));
            mol.Atoms[0].FormalCharge = -2;
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.Atoms.Add(new Atom("F"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Be.2minus", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestArsine()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("As"));
            mol.Atoms.Add(new Atom("H"));
            mol.Atoms.Add(new Atom("H"));
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "As", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBoron()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("B"));
            mol.Atoms.Add(new Atom("H"));
            mol.Atoms.Add(new Atom("H"));
            mol.Atoms.Add(new Atom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "B", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbonMonoxide()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.minus.sp1", "O.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTitaniumFourCoordinate()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Ti"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Ti.sp3", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1872969
        [TestMethod()]
        public void Bug1872969()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms.Add(new Atom("S"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms[4].FormalCharge = -1;
            mol.Atoms.Add(new Atom("Na"));
            mol.Atoms[5].FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "S.onyl", "O.sp2", "O.sp2", "O.minus", "Na.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

         /// <summary>
         /// Test if all elements up to and including Uranium have atom types.
         /// </summary>
        [TestMethod()]
        public void TestAllElementsRepresented()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl",
                    Silent.ChemObjectBuilder.Instance);
            Assert.IsTrue(factory.Count != 0, "Could not read the atom types");
            string errorMessage = "Elements without atom type(s) defined in the XML:";
            int testUptoAtomicNumber = 36; // TODO: 92 ?
            int elementsMissingTypes = 0;
            for (int i = 1; i < testUptoAtomicNumber; i++)
            {
                string symbol = PeriodicTable.GetSymbol(i);
                var expectedTypes = factory.GetAtomTypes(symbol);
                if (expectedTypes.Count() == 0)
                {
                    errorMessage += " " + symbol;
                    elementsMissingTypes++;
                }
            }
            Assert.AreEqual(0, elementsMissingTypes, errorMessage);
        }

        [TestMethod()]
        public void TestAssumeExplicitHydrogens()
        {
            IAtomContainer mol = new AtomContainer();
            CDKAtomTypeMatcher atm = CDKAtomTypeMatcher.GetInstance(mol.Builder,
                    CDKAtomTypeMatcher.RequireExplicitHydrogens);

            mol.Atoms.Add(new Atom("O"));
            mol.Atoms[0].FormalCharge = +1;
            IAtomType type = atm.FindMatchingAtomType(mol, mol.Atoms[0]);
            Assert.IsNotNull(type);
            Assert.AreEqual("X", type.AtomTypeName);

            for (int i = 0; i < 3; i++)
            {
                mol.Atoms.Add(new Atom("H"));
                mol.Bonds.Add(new Bond(mol.Atoms[i + 1], mol.Atoms[0], BondOrder.Single));
            }
            AssertAtomType(testedAtomTypes, "O.plus", atm.FindMatchingAtomType(mol, mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestStructGenMatcher()
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestCarbonRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            IAtom atom4 = new Atom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "C.radical.planar", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1382 
        [TestMethod()]
        public void TestCarbonDiradical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            IAtomTypeMatcher atm = GetAtomTypeMatcher(mol.Builder);
            IAtomType foundType = atm.FindMatchingAtomType(mol, atom);
            Assert.AreEqual("X", foundType.AtomTypeName);
        }

        [TestMethod()]
        public void TestEthoxyEthaneRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.plus.radical", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylFluorRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "F.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylChloroRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Cl");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Cl.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylBromoRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Br.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylIodoRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "I.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneFluorKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "F.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneChlorKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Cl");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "Cl.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneBromKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "Br.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneIodKation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "I.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethanolRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            IAtom atom2 = new Atom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "O.sp3.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylMethylimineRadical()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("N");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp2.radical", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChargeSeparatedFluoroEthane()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("F");
            IAtom atom2 = new Atom("C");
            atom2.FormalCharge = +1;
            IAtom atom3 = new Atom("C");
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "F", "C.plus.planar", "C.minus.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C2H7NS/c1-4(2)3/h3H,1-2H3
        [TestMethod()]
        public void TestSulphurCompound()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("N");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "S.inyl", "N.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAluminumChloride()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("Cl");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("Cl");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("Cl");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("Al");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Cl", "Cl", "Cl", "Al" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C3H9NO/c1-4(2,3)5/h1-3H3
        [TestMethod()]
        public void Cid1145()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(a1);
            a1.FormalCharge = -1;
            IAtom a2 = mol.Builder.CreateAtom("N");
            mol.Atoms.Add(a2);
            a2.FormalCharge = +1;
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a14);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a3, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a4, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a4, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a4, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a5, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a5, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a5, a14, BondOrder.Single);
            mol.Bonds.Add(b13);

            string[] expectedTypes = {"O.minus", "N.plus", "C.sp3", "C.sp3", "C.sp3", "H", "H", "H", "H", "H", "H", "H",
                "H", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChiPathFail()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C6H5IO/c8-7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestIodosobenzene()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            IAtom iodine = mol.Builder.CreateAtom("I");
            IAtom oxygen = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(iodine);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "I.3", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C6H5IO2/c8-7(9)6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestIodoxybenzene()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            IAtom iodine = mol.Builder.CreateAtom("I");
            IAtom oxygen1 = mol.Builder.CreateAtom("O");
            IAtom oxygen2 = mol.Builder.CreateAtom("O");
            mol.Atoms.Add(iodine);
            mol.Atoms.Add(oxygen1);
            mol.Atoms.Add(oxygen2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);
            mol.AddBond(mol.Atoms[6], mol.Atoms[8], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "I.5", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C7H7NOS/c8-7(10-9)6-4-2-1-3-5-6/h1-5H,8H2
        [TestMethod()]
        public void TestThiobenzamideSOxide()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBenzene();
            IAtom carbon = mol.Builder.CreateAtom("C");
            IAtom sulphur = mol.Builder.CreateAtom("S");
            IAtom oxygen = mol.Builder.CreateAtom("O");
            IAtom nitrogen = mol.Builder.CreateAtom("N");
            mol.Atoms.Add(carbon);
            mol.Atoms.Add(sulphur);
            mol.Atoms.Add(oxygen);
            mol.Atoms.Add(nitrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Double);
            mol.AddBond(mol.Atoms[6], mol.Atoms[9], BondOrder.Single);

            string[] expectedTypes = {"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "S.inyl.2", "O.sp2",
                "N.thioamide"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C4H10S/c1-5(2)3-4-5/h3-4H2,1-2H3
        [TestMethod()]
        public void TestDimethylThiirane()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("S"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp3", "C.sp3", "S.anyl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi     InChI=1/C3H8S/c1-4(2)3/h1H2,2-3H3
        [TestMethod()]
        public void TestSulphonylLookalike()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("C"));
            mol.Atoms.Add(mol.Builder.CreateAtom("S"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp2", "S.inyl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNOxide()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            IAtom a2 = mol.Builder.CreateAtom("C");
            IAtom a3 = mol.Builder.CreateAtom("N");
            IAtom a4 = mol.Builder.CreateAtom("O");
            IAtom a5 = mol.Builder.CreateAtom("O");

            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Atoms.Add(a5);

            mol.Bonds.Add(mol.Builder.CreateBond(a1, a2, BondOrder.Single));
            mol.Bonds.Add(mol.Builder.CreateBond(a2, a3, BondOrder.Single));
            mol.Bonds.Add(mol.Builder.CreateBond(a3, a4, BondOrder.Double));
            mol.Bonds.Add(mol.Builder.CreateBond(a3, a5, BondOrder.Double));

            string[] expectedTypes = { "C.sp3", "C.sp3", "N.nitro", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestGermaniumFourCoordinate()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Ge"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Ge", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPlatinumFourCoordinate()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Pt"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Pt.4", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPlatinumSixCoordinate()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("Pt"));
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(new Atom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "Pt.6", "Cl", "Cl", "Cl", "Cl", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 2424511
        [TestMethod()]
        public void TestWeirdNitrogen()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms.Add(new Atom("N"));
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms.Add(new Atom("C"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "N.sp1.2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

         /// <summary>
         /// Testing a nitrogen as found in this SMILES input: c1c2cc[nH]cc2nc1.
         /// </summary>
        [TestMethod()]
        public void TestAnotherNitrogen()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[0].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[1].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[2].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[3].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("N"));
            mol.Atoms[4].Hybridization = Hybridization.Planar3;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[5].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[6].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("N"));
            mol.Atoms[7].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[8].Hybridization = Hybridization.SP2;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[8], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "N.planar3", "C.sp2", "C.sp2", "N.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3061263
        [TestMethod()]
        public void TestFormalChargeRepresentation()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("O");
            Hybridization thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);
            string[] expectedTypes = { "O.minus" };

            // option one: int.Parse()
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            // option one: autoboxing
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            // option one: new Integer()
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3190151
        [TestMethod()]
        public void TestP()
        {
            IAtom atomP = new Atom("P");
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(atomP);
            string[] expectedTypes = { "P.ine" };

            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3190151
        [TestMethod()]
        public void TestPine()
        {
            IAtom atomP = new Atom(Elements.Phosphorus.ToIElement());
            IAtomType atomTypeP = new AtomType(Elements.Phosphorus.ToIElement());
            AtomTypeManipulator.Configure(atomP, atomTypeP);

            IAtomContainer ac = atomP.Builder.CreateAtomContainer();
            ac.Atoms.Add(atomP);
            IAtomType type = null;
            foreach (var atom in ac.Atoms)
            {
                type = CDKAtomTypeMatcher.GetInstance(ac.Builder).FindMatchingAtomType(ac, atom);
                Assert.IsNotNull(type);
            }
        }

        [TestMethod()]
        public void Test_S_sp3d1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "S.sp3d1", "C.sp3", "C.sp3", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_inyl_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.inyl.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_2minus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "S.2minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_sp3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_sp3_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "S.sp3.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Co.plus.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Co.plus.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("Co");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "C.sp3", "Co.2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Co.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Co.plus.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Co.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_5()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "Co.plus.5", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3529082
        [TestMethod()]
        public void Test_Co_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a2 = builder.CreateAtom("Co");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);

            string[] expectedTypes = { "Co.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Co");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Co.plus.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Co.1", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Bromic acid (CHEBI:49382).
        /// </summary>
        // @cdk.inchi InChI=1S/BrHO3/c2-1(3)4/h(H,2,3,4)
        [TestMethod()]
        public void Test_Br_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Br");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("O");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("O");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("O");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Br.3", "O.sp2", "O.sp2", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Zn_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Zn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Zn.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Zn_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Zn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Zn.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Vanadate. PDB HET ID : VO4.
        /// </summary>
        // @cdk.inchi InChI=1S/4O.V/q;3*-1;
        [TestMethod()]
        public void Test_V_3minus_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("V");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("O");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("O");
            a3.FormalCharge = -1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("O");
            a4.FormalCharge = -1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("O");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "V.3minus.4", "O.minus", "O.minus", "O.minus", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Hexafluoroaluminate
        /// </summary>
        // @cdk.inchi InChI=1S/Al.6FH.3Na/h;6*1H;;;/q+3;;;;;;;3*+1/p-6
        [TestMethod()]
        public void Test_Al_3minus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Al");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Al.3minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSe_sp3d1_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes1 = { "Se.sp3d1.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void TestSe_sp3_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Se.sp3.4", "C.sp3", "C.sp3", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSe_sp2_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes2 = { "Se.sp2.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes2, mol);
        }

        [TestMethod()]
        public void TestSe_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Se");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes3 = { "C.sp2", "Se.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes3, mol);
        }

        [TestMethod()]
        public void TestSe_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes4 = { "Se.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes4, mol);
        }

        [TestMethod()]
        public void TestSe_sp3_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Se");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a2, a4, BondOrder.Double);
            mol.Bonds.Add(b3);

            string[] expectedTypes5 = { "C.sp3", "Se.sp3.3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes5, mol);
        }

        [TestMethod()]
        public void TestSe_4plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 4;
            mol.Atoms.Add(a1);

            string[] expectedTypes6 = { "Se.4plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes6, mol);
        }

        [TestMethod()]
        public void TestSe_plus_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Se");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes7 = { "C.sp3", "Se.plus.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes7, mol);
        }

        [TestMethod()]
        public void TestSe_5()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes8 = { "Se.5", "C.sp2", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes8, mol);
        }

        [TestMethod()]
        public void Test_Se_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Se");
            a1.ImplicitHydrogenCount = 0;
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Se.2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/H2Te/h1H2
        [TestMethod()]
        public void TestTellane()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Te");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("H");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("H");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Te.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C3H6P/c1-3-4-2/h3H,2H2,1H3/q+1
        [TestMethod()]
        public void TestPhosphanium()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("P");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "P.sp1.plus", "C.sp3", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/CHP/c1-2/h1H
        [TestMethod()]
        public void TestPhosphide()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("H");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Triple);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "P.ide", "C.sp", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPentaMethylPhosphane()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "P.ane", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sb_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Sb");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a2, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "Sb.4", "C.sp3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sb_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Sb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Sb.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_B_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("B");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "B.3plus", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sr_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Sr");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Sr.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Te_4plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Te");
            a1.FormalCharge = 4;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Te.4plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Be_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Be");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Be.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cl_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cl");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Cl.2", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_K_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("K");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "K.neutral", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Li_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Li");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Li.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Li_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Li");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Li.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_I_sp3d2_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("I");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "I.sp3d2.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public override void TestForDuplicateDefinitions()
        {
            base.TestForDuplicateDefinitions();
        }

        // @cdk.inchi InChI=1S/CH2N2/c1-3-2/h1H2
        [TestMethod()]
        public void TestAzoCompound()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("N");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("N");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("H");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("H");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = mol.Builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "N.plus.sp1", "N.minus.sp2", "C.sp2", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug   3141611
        // @cdk.inchi InChI=1S/CH5O2P/c1-4(2)3/h4H,1H3,(H,2,3)
        [TestMethod()]
        public void TestMethylphosphinicAcid()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("O");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("O");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("H");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("H");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("H");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.CreateAtom("H");
            a8.FormalCharge = 0;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.CreateAtom("H");
            a9.FormalCharge = 0;
            mol.Atoms.Add(a9);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a2, a9, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a4, a6, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.CreateBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.CreateBond(a4, a8, BondOrder.Single);
            mol.Bonds.Add(b8);

            string[] expectedTypes = { "P.ate", "O.sp3", "O.sp2", "C.sp3", "H", "H", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ti_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ti");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Ti.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ni_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ni");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ni.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ni_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Ni");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Ni.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Pb.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pb");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pb.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pb.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Tl");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Tl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Tl");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Tl.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Tl");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Tl.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Mg");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "Mg.neutral.2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("Mg");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "C.sp3", "Mg.neutral", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Mg.neutral.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Gd_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Gd");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Gd.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mo_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mo");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Mo.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mo_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mo");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mo.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pt_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pt");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Pt.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pt_2plus_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pt");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Pt.2plus.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cu.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cu");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cu.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Cu.1", "C.sp3", };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ra()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ra");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ra.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cr");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Rb_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Rb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Rb.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Rb_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Rb");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Rb.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cr");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Cr.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cr");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_6plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cr");
            a1.FormalCharge = 6;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.6plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ba_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ba");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ba.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Au_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Au");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Au.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ag_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ag");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ag.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// For example PubChem CID 3808730.
        /// </summary>
        [TestMethod()]
        public void Test_Ag_plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ag");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ag.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// For example PubChem CID 139654.
        /// </summary>
        [TestMethod()]
        public void Test_Ag_covalent()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ag");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Cl");
            mol.Atoms.Add(a2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "Ag.1", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("In");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "In.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "In.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_1()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Triple);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "In.1", "C.sp" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "In" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_2plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cd");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cd.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cd");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Cd.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Cd");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cd.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pu()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Pu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pu" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Th()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Th");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Th" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ge_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Ge");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "C.sp3", "Ge.3", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Na_neutral()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Na");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Na.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mn");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mn.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Mn.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_metallic()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Mn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mn.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_2minus_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Si");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Si.2minus.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_3()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Si");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Si.3", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Si");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Si.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_minus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("As");
            a1.FormalCharge = -1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "As.minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_3plus()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("As");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "As.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_2()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("As");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "As.2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_5()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("As");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "As.5", "C.sp3", "C.sp3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Fe_metallic()
        {
            //string molName = "Fe_metallic";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            string[] expectedTypes = { "Fe.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Fe_plus()
        {
            //string molName1 = "Fe_plus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("Fe");
            a3.FormalCharge = 1;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes1 = { "C.sp3", "C.sp3", "Fe.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void Test_Fe_4()
        {
            //string molName2 = "Fe_4";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("Fe");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            string[] expectedTypes2 = { "C.sp3", "C.sp3", "Fe.4", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes2, mol);
        }

        [TestMethod()]
        public void Test_Fe_3minus()
        {
            //string molName3 = "Fe_3minus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes3 = { "Fe.3minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes3, mol);
        }

        [TestMethod()]
        public void Test_Fe_2plus()
        {
            //string molName4 = "Fe_2plus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);
            string[] expectedTypes4 = { "Fe.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes4, mol);
        }

        [TestMethod()]
        public void Test_Fe_4minus()
        {
            //string molName5 = "Fe_4minus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = -4;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes5 = { "Fe.4minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes5, mol);
        }

        [TestMethod()]
        public void Test_Fe_5()
        {
            //string molNameFe5 = "Fe_5";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            string[] expectedTypesFe5 = { "Fe.5", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesFe5, mol);
        }

        [TestMethod()]
        public void Test_Fe_6()
        {
            //string molName7 = "Fe_6";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes7 = { "Fe.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes7, mol);
        }

        [TestMethod()]
        public void Test_Fe_2minus()
        {
            //string molName8 = "Fe_2minus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes8 = { "Fe.2minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes8, mol);
        }

        [TestMethod()]
        public void Test_Fe_3plus()
        {
            //string molName9 = "Fe_3plus";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);
            string[] expectedTypes9 = { "Fe.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes9, mol);
        }

        [TestMethod()]
        public void Test_Fe_2()
        {
            //string molNameA = "Fe_2";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("Fe");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            string[] expectedTypesA = { "C.sp3", "Fe.2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesA, mol);
        }

        [TestMethod()]
        public void Test_Fe_3()
        {
            //string molNameB = "Fe_3";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            string[] expectedTypesB = { "Fe.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesB, mol);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C8H16S/c1-6-3-8-4-7(6)5-9(8)2/h6-9H,3-5H2,1-2H3/t6-,7-,8+/m0/s1
        /// </summary>
        [TestMethod()]
        public void TestSulphur4()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            mol.Atoms.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            mol.Atoms.Add(a9);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b5 = builder.CreateBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b8 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.CreateBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b14 = builder.CreateBond(a5, a7, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = builder.CreateBond(a5, a9, BondOrder.Single);
            mol.Bonds.Add(b15);

            string[] expectedTypes = { "S.anyl", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// One of the ruthenium atom types in ruthenium red (CHEBI:34956).
        /// </summary>
        [TestMethod()]
        public void Test_Ru_3minus_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ru");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("N");
            a3.FormalCharge = +1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("N");
            a4.FormalCharge = +1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("N");
            a5.FormalCharge = +1;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("N");
            a6.FormalCharge = +1;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("O");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.3minus.6", "N.plus", "N.plus", "N.plus", "N.plus", "N.plus", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// One of the ruthenium atom types in ruthenium red (CHEBI:34956).
        /// </summary>
        [TestMethod()]
        public void Test_Ru_2minus_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ru");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("N");
            a3.FormalCharge = +1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("N");
            a4.FormalCharge = +1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("N");
            a5.FormalCharge = +1;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("O");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("O");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.2minus.6", "N.plus", "N.plus", "N.plus", "N.plus", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ru_10plus_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ru");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ru_6()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("Ru");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a7, a1, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C4H5N/c1-2-4-5-3-1/h1-5H
        [TestMethod()]
        public void Test_n_planar3_sp2_aromaticity()
        {

            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

            // simulate an IAtomContainer returned from a SDFile with bond order 4 to indicate aromaticity
            IAtomContainer pyrrole = builder.CreateAtomContainer();

            IAtom n1 = builder.CreateAtom("N");
            IAtom c2 = builder.CreateAtom("C");
            IAtom c3 = builder.CreateAtom("C");
            IAtom c4 = builder.CreateAtom("C");
            IAtom c5 = builder.CreateAtom("C");

            IBond b1 = builder.CreateBond(n1, c2, BondOrder.Single);
            b1.IsAromatic = true;
            IBond b2 = builder.CreateBond(c2, c3, BondOrder.Single);
            b2.IsAromatic = true;
            IBond b3 = builder.CreateBond(c3, c4, BondOrder.Single);
            b3.IsAromatic = true;
            IBond b4 = builder.CreateBond(c4, c5, BondOrder.Single);
            b4.IsAromatic = true;
            IBond b5 = builder.CreateBond(c5, n1, BondOrder.Single);
            b5.IsAromatic = true;

            pyrrole.Atoms.Add(n1);
            pyrrole.Atoms.Add(c2);
            pyrrole.Atoms.Add(c3);
            pyrrole.Atoms.Add(c4);
            pyrrole.Atoms.Add(c5);
            pyrrole.Bonds.Add(b1);
            pyrrole.Bonds.Add(b2);
            pyrrole.Bonds.Add(b3);
            pyrrole.Bonds.Add(b4);
            pyrrole.Bonds.Add(b5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pyrrole);

            Assert.AreEqual(pyrrole.Atoms[0].Hybridization.Name, "Planar3");
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C4H5N/c1-2-4-5-3-1/h1-5H
        // @
        /// </summary>
        [TestMethod()]
        public void Test_n_planar3_sp2_aromaticity_explicitH()
        {

            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

            // simulate an IAtomContainer returned from a SDFile with bond order 4 to indicate aromaticity
            IAtomContainer pyrrole = builder.CreateAtomContainer();

            IAtom n1 = builder.CreateAtom("N");
            IAtom c2 = builder.CreateAtom("C");
            IAtom c3 = builder.CreateAtom("C");
            IAtom c4 = builder.CreateAtom("C");
            IAtom c5 = builder.CreateAtom("C");

            IBond b1 = builder.CreateBond(n1, c2, BondOrder.Single);
            b1.IsAromatic = true;
            IBond b2 = builder.CreateBond(c2, c3, BondOrder.Single);
            b2.IsAromatic = true;
            IBond b3 = builder.CreateBond(c3, c4, BondOrder.Single);
            b3.IsAromatic = true;
            IBond b4 = builder.CreateBond(c4, c5, BondOrder.Single);
            b4.IsAromatic = true;
            IBond b5 = builder.CreateBond(c5, n1, BondOrder.Single);
            b5.IsAromatic = true;

            pyrrole.Atoms.Add(n1);
            pyrrole.Atoms.Add(c2);
            pyrrole.Atoms.Add(c3);
            pyrrole.Atoms.Add(c4);
            pyrrole.Atoms.Add(c5);
            pyrrole.Bonds.Add(b1);
            pyrrole.Bonds.Add(b2);
            pyrrole.Bonds.Add(b3);
            pyrrole.Bonds.Add(b4);
            pyrrole.Bonds.Add(b5);

            // add explicit hydrogens
            IAtom h1 = builder.CreateAtom("H");
            IAtom h2 = builder.CreateAtom("H");
            IAtom h3 = builder.CreateAtom("H");
            IAtom h4 = builder.CreateAtom("H");
            IAtom h5 = builder.CreateAtom("H");
            pyrrole.Atoms.Add(h1);
            pyrrole.Atoms.Add(h2);
            pyrrole.Atoms.Add(h3);
            pyrrole.Atoms.Add(h4);
            pyrrole.Atoms.Add(h5);
            pyrrole.Bonds.Add(builder.CreateBond(n1, h1));
            pyrrole.Bonds.Add(builder.CreateBond(c2, h2));
            pyrrole.Bonds.Add(builder.CreateBond(c3, h3));
            pyrrole.Bonds.Add(builder.CreateBond(c4, h4));
            pyrrole.Bonds.Add(builder.CreateBond(c5, h5));

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pyrrole);

            Assert.AreEqual(pyrrole.Atoms[0].Hybridization.Name, "Planar3");
        }

        [ClassCleanup()]
        public static void TestTestedAtomTypes()
        {
            CountTestedAtomTypes(testedAtomTypes, factory);
        }
    }
}