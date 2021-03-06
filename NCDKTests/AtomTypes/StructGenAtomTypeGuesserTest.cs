/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.AtomTypes
{
    // @cdk.module test-structgen
    [TestClass()]
    public class StructGenAtomTypeGuesserTest
        : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestPossibleAtomTypes_IAtomContainer_IAtom()
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom("C");
            atom.ImplicitHydrogenCount = 3;
            var atom2 = builder.NewAtom("N");
            atom.ImplicitHydrogenCount = 2;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Bonds.Add(builder.NewBond(atom, atom2, BondOrder.Single));

            StructGenAtomTypeGuesser atm = new StructGenAtomTypeGuesser();
            var matched = atm.PossibleAtomTypes(mol, atom);
            Assert.IsNotNull(matched);
            Assert.IsTrue(matched.Count() > 0);
            Assert.IsTrue(matched.ElementAt(0) is IAtomType);

            Assert.AreEqual("C", ((IAtomType)matched.ElementAt(0)).Symbol);
        }

        [TestMethod()]
        public void TestStructGenAtomTypeGuesser()
        {
            StructGenAtomTypeGuesser matcher = new StructGenAtomTypeGuesser();
            Assert.IsNotNull(matcher);
        }
    }
}
