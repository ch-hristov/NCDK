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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the SingleElectron class.
    /// </summary>
    // @see org.openscience.cdk.SingleElectron
    // @cdk.module test-data
    [TestClass()]
    public class SingleElectronTest : AbstractSingleElectronTest
    {
        public override IChemObject NewChemObject()
        {
            return new SingleElectron();
        }

        [TestMethod()]
        public virtual void TestSingleElectron()
        {
            ISingleElectron radical = new SingleElectron();
            Assert.IsNull(radical.Atom);
            Assert.AreEqual(1, radical.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestSingleElectron_IAtom()
        {
            IAtom atom = NewChemObject().Builder.CreateAtom("N");
            ISingleElectron radical = new SingleElectron(atom);
            Assert.AreEqual(1, radical.ElectronCount.Value);
            Assert.AreEqual(atom, radical.Atom);
            Assert.IsTrue(radical.Contains(atom));
        }
    }
}