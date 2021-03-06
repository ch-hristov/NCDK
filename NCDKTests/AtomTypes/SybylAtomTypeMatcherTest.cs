/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the perception of Sybyl atom types, which uses
    /// CDK atom type perception and mapping of CDK atom types to Sybyl
    /// atom types.
    /// </summary>
    // @cdk.module test-atomtype
    [TestClass()]
    public class SybylAtomTypeMatcherTest : AbstractSybylAtomTypeTest
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        private static readonly IDictionary<string, int> testedAtomTypes = new Dictionary<string, int>
        {
            // do not complain about a few non-tested atom types
            // so, just mark them as tested
            ["LP"] = 1,
            ["Du"] = 1,
            ["Du.C"] = 1,
            ["Any"] = 1,
            ["Hal"] = 1,
            ["Het"] = 1,
            ["Hev"] = 1,
            ["X"] = 1,
            ["Het"] = 1,
            ["H.t3p"] = 1,
            ["H.spc"] = 1,
            ["O.t3p"] = 1,
            ["O.spc"] = 1,
        };

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            var matcher = SybylAtomTypeMatcher.GetInstance();
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer_IAtom()
        {
            var matcher = SybylAtomTypeMatcher.GetInstance();
            Assert.IsNotNull(matcher);
            var ethane = TestMoleculeFactory.MakeAlkane(2);
            var expectedTypes = new[] { "C.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, ethane);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer()
        {
            var filename = "NCDK.Data.Mol2.atomtyping.mol2";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new Mol2Reader(ins);
            var mol = reader.Read(builder.NewAtomContainer());

            // just check consistency; other methods do perception testing
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var types = matcher.FindMatchingAtomTypes(mol).ToReadOnlyList();
            for (int i = 0; i < types.Count; i++)
            {
                IAtomType type = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                Assert.AreEqual(type.AtomTypeName, types[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestAtomTyping()
        {
            var filename = "NCDK.Data.Mol2.atomtyping.mol2";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new Mol2Reader(ins);
            var molecule = (IAtomContainer)reader.Read(builder.NewAtomContainer());
            Assert.IsNotNull(molecule);
            var reference = (IAtomContainer)molecule.Clone();

            // test if the perceived atom types match that
            PercieveAtomTypesAndConfigureAtoms(molecule);
            var refAtoms = reference.Atoms.GetEnumerator();
            var atoms = molecule.Atoms.GetEnumerator();
            while (atoms.MoveNext() && refAtoms.MoveNext())
            {
                // work around aromaticity, which we skipped for now
                Assert.AreEqual(refAtoms.Current.AtomTypeName, atoms.Current.AtomTypeName, 
                    "Perceived atom type does not match atom type in file");
            }
        }

        /// <summary>
        /// Uses FindMatchingAtomType(IAtomContainer, IAtom) type.
        /// </summary>
        [TestMethod()]
        public void TestBenzene()
        {
            var benzene = TestMoleculeFactory.MakeBenzene();

            // test if the perceived atom types match that
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var types = matcher.FindMatchingAtomTypes(benzene);
            foreach (var type in types)
            {
                Assert.AreEqual("C.ar", type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestAdenine()
        {
            var mol = TestMoleculeFactory.MakeAdenine();
            var expectedTypes = new[] { "C.ar", "C.ar", "C.ar", "N.ar", "N.ar", "N.ar", "N.ar", "N.3", "C.ar", "C.ar" };
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var types = matcher.FindMatchingAtomTypes(mol).ToReadOnlyList();
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                AssertAtomType(testedAtomTypes, $"Incorrect perception for atom {i}", expectedTypes[i], types[i]);
            }
        }

        /// <summary>
        /// Uses FindMatchingAtomType(IAtomContainer) type.
        /// </summary>
        [TestMethod()]
        public void TestBenzene_AtomContainer()
        {
            var benzene = TestMoleculeFactory.MakeBenzene();

            // test if the perceived atom types match that
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var types = matcher.FindMatchingAtomTypes(benzene);
            foreach (var type in types)
            {
                Assert.AreEqual("C.ar", type.AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestAtomTyping4()
        {
            var filename = "NCDK.Data.Mol2.atomtyping4.mol2";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new Mol2Reader(ins);
            var molecule = (IAtomContainer)reader.Read(builder.NewAtomContainer());
            Assert.IsNotNull(molecule);
            var reference = (IAtomContainer)molecule.Clone();

            // test if the perceived atom types match that
            PercieveAtomTypesAndConfigureAtoms(molecule);
            var refAtoms = reference.Atoms.GetEnumerator();
            var atoms = molecule.Atoms.GetEnumerator();
            while (atoms.MoveNext() && refAtoms.MoveNext())
            {
                // work around aromaticity, which we skipped for now
                IAtom refAtom = refAtoms.Current;
                Assert.AreEqual(refAtom.AtomTypeName,
                        atoms.Current.AtomTypeName,
                        "Perceived atom type does not match atom type in file");
            }
        }

        /// <summary>
        // @cdk.bug 2445178
        /// </summary>
        [TestMethod()]
        public void TestNonExistingType()
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom();
            mol.Atoms.Add(atom);
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            Assert.AreEqual("X", type.AtomTypeName);
        }

        [TestMethod()]
        public void TestAtomTyping2()
        {
            var filename = "NCDK.Data.Mol2.atomtyping2.mol2";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new Mol2Reader(ins);
            var molecule = reader.Read(builder.NewAtomContainer());
            Assert.IsNotNull(molecule);
            var reference = (IAtomContainer)molecule.Clone();

            // test if the perceived atom types match that
            PercieveAtomTypesAndConfigureAtoms(molecule);
            var refAtoms = reference.Atoms.GetEnumerator();
            var atoms = molecule.Atoms.GetEnumerator();
            while (atoms.MoveNext() && refAtoms.MoveNext())
            {
                // work around aromaticity, which we skipped for now
                IAtom refAtom = refAtoms.Current;
                Assert.AreEqual(refAtom.AtomTypeName,
                        atoms.Current.AtomTypeName,
                        "Perceived atom type does not match atom type in file");
            }
        }

        [TestMethod()]
        public void TestAtomTyping3()
        {
            var filename = "NCDK.Data.Mol2.atomtyping3.mol2";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new Mol2Reader(ins);
            var molecule = (IAtomContainer)reader.Read(builder.NewAtomContainer());
            Assert.IsNotNull(molecule);
            var reference = (IAtomContainer)molecule.Clone();

            // test if the perceived atom types match that
            PercieveAtomTypesAndConfigureAtoms(molecule);
            var refAtoms = reference.Atoms.GetEnumerator();
            var atoms = molecule.Atoms.GetEnumerator();
            while (atoms.MoveNext() && refAtoms.MoveNext())
            {
                // work around aromaticity, which we skipped for now
                IAtom refAtom = refAtoms.Current;
                Assert.AreEqual(refAtom.AtomTypeName,
                        atoms.Current.AtomTypeName,
                        "Perceived atom type does not match atom type in file");
            }
        }

        private static void PercieveAtomTypesAndConfigureAtoms(IAtomContainer container)
        {
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var atoms = container.Atoms.GetEnumerator();
            while (atoms.MoveNext())
            {
                var atom = atoms.Current;
                atom.AtomTypeName = null;
                var matched = matcher.FindMatchingAtomType(container, atom);
                if (matched != null)
                    AtomTypeManipulator.Configure(atom, matched);
            }
        }

        [TestMethod()]
        public override void TestForDuplicateDefinitions()
        {
            base.TestForDuplicateDefinitions();
        }

        [TestMethod()]
        public void TestDummy()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewPseudoAtom("R");
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "X" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthene()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.2", "C.2" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestImine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.2", "N.2" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPropyne()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.1", "C.1", "C.3" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAllene()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.2", "C.1", "C.2" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHalogenatedMethane()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.Atoms.Add(builder.NewAtom("I"));
            mol.Atoms.Add(builder.NewAtom("Br"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.3", "F", "Cl", "I", "Br" };
            AssertAtomTypeNames(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMnF4()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("Mn");
            IAtom atom3 = builder.NewAtom("F");
            IAtom atom4 = builder.NewAtom("F");
            IAtom atom5 = builder.NewAtom("F");
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

            string[] expectedTypes = { "F", "Mn", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.2", "C.2", "N.am" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarboxylicAcid()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.co2", "C.2", "O.co2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarboxylate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("O");
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.co2", "C.2", "O.co2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylAmine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylNitro_Charged()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            IAtom atom2 = builder.NewAtom("N");
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("O");
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            IAtom atom4 = builder.NewAtom("O");
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = { "C.3", "N.pl3", "O.3", "O.2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmmonia()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("H");
            IAtom atom4 = builder.NewAtom("H");
            IAtom atom5 = builder.NewAtom("H");
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

            string[] expectedTypes = { "H", "N.4", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethanol()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSO()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.2", "S.O", "C.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSOO()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.2", "O.2", "S.O2", "C.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbokation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("C");
            atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("H");
            IAtom atom4 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "C.cat", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSilicon()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("Si");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a17);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a5, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a5, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a6, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.NewBond(a6, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.NewBond(a6, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.NewBond(a7, a14, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.NewBond(a7, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.NewBond(a7, a16, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.NewBond(a1, a17, BondOrder.Single);
            mol.Bonds.Add(b16);

            string[] expectedTypes = {"Si", "O.3", "O.3", "O.3", "C.3", "C.3", "C.3", "H", "H", "H", "H", "H", "H", "H",
                "H", "H", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestThioAmide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("S");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "S.2", "C.2", "N.am" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSalts()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("Na");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            string[] expectedTypes = new string[] { "Na" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("K");
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "K" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Ca");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Ca" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Mg");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Mg" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Cu");
            atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Cu" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Al");
            atom.FormalCharge = +3;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Al" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestH2S()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom s = builder.NewAtom("S");
            IAtom h1 = builder.NewAtom("H");
            IAtom h2 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(s, h1, BondOrder.Single);
            IBond b2 = builder.NewBond(s, h2, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFerrocene()
        {
            IAtomContainer ferrocene = builder.NewAtomContainer();
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms[4].FormalCharge = -1;
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms[9].FormalCharge = -1;
            ferrocene.Atoms.Add(builder.NewAtom("Fe"));
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

            var expectedTypes = new string[] {"C.2", "C.2", "C.2", "C.2", "Any", "C.2", "C.2", "C.2", "C.2", "Any", "Fe"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, ferrocene);
        }

        [TestMethod()]
        public void TestHCN()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            var expectedTypes = new[] { "N.1", "C.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAniline()
        {
            var benzene = TestMoleculeFactory.MakeBenzene();
            var nitrogen = benzene.Builder.NewAtom("N");
            benzene.Atoms.Add(nitrogen);
            benzene.Bonds.Add(benzene.Builder.NewBond(benzene.Atoms[0], nitrogen, BondOrder.Single));

            // test if the perceived atom types match that
            var matcher = SybylAtomTypeMatcher.GetInstance();
            var types = matcher.FindMatchingAtomTypes(benzene).ToReadOnlyList();
            for (int i = 0; i < 6; i++)
            {
                AssertAtomType(testedAtomTypes, "Incorrect perception for atom " + i, "C.ar", types[i]);
            }
            AssertAtomType(testedAtomTypes, "Incorrect perception for atom " + 6, "N.3", types[6]);
        }

        [TestMethod()]
        public void TestLithiumMethanoxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("Li");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.3", "C.3", "Li" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTinCompound()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Sn");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.3", "Sn", "C.3", "C.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestZincChloride()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Zn"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Zn", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        // @cdk.inchi InChI=1/H2Se/h1H2
        /// </summary>
        [TestMethod()]
        public void TestH2Se()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom se = builder.NewAtom("Se");
            IAtom h1 = builder.NewAtom("H");
            IAtom h2 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(se, h1, BondOrder.Single);
            IBond b2 = builder.NewBond(se, h2, BondOrder.Single);

            mol.Atoms.Add(se);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Se", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("P");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.2", "P.3", "O.3", "O.3", "O.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mo_4()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mo");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            mol.Atoms.Add(a5);
            mol.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Double));
            mol.Bonds.Add(builder.NewBond(a1, a3, BondOrder.Double));
            mol.Bonds.Add(builder.NewBond(a1, a4, BondOrder.Single));
            mol.Bonds.Add(builder.NewBond(a1, a5, BondOrder.Single));

            string[] expectedTypes = { "Mo", "C.2", "C.2", "C.3", "C.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCrth()
        {
            var mol = builder.NewAtomContainer();
            // this is made up
            IAtom a1 = builder.NewAtom("Cr");
            mol.Atoms.Add(a1);
            for (int i = 0; i < 4; i++)
            {
                IAtom atom = builder.NewAtom("O");
                mol.Atoms.Add(atom);
                mol.Bonds.Add(builder.NewBond(a1, atom, BondOrder.Single));
            }

            string[] expectedTypes = { "Cr.th", "O.3", "O.3", "O.3", "O.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCroh()
        {
            var mol = builder.NewAtomContainer();
            // this is made up, and may be wrong; info on the web is sparse, and PubChem has no
            // octa-coordinate structure; lone pairs involved?
            IAtom a1 = builder.NewAtom("Cr");
            mol.Atoms.Add(a1);
            for (int i = 0; i < 6; i++)
            {
                IAtom atom = builder.NewAtom("O");
                mol.Atoms.Add(atom);
                mol.Bonds.Add(builder.NewBond(a1, atom, BondOrder.Single));
            }

            string[] expectedTypes = { "Cr.oh", "O.3", "O.3", "O.3", "O.3", "O.3", "O.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCooh()
        {
            var mol = builder.NewAtomContainer();
            // this is made up, and may be wrong; info on the web is sparse, and PubChem has no
            // octa-coordinate structure; lone pairs involved?
            IAtom a1 = builder.NewAtom("Co");
            mol.Atoms.Add(a1);
            for (int i = 0; i < 6; i++)
            {
                IAtom atom = builder.NewAtom("O");
                mol.Atoms.Add(atom);
                mol.Bonds.Add(builder.NewBond(a1, atom, BondOrder.Single));
            }

            string[] expectedTypes = { "Co.oh", "O.3", "O.3", "O.3", "O.3", "O.3", "O.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }
    }
}
