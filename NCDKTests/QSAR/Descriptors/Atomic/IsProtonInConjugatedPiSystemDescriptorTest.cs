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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class IsProtonInConjugatedPiSystemDescriptorTest : AtomicDescriptorTest
    {
        public IsProtonInConjugatedPiSystemDescriptorTest()
        {
            SetDescriptor(typeof(IsProtonInConjugatedPiSystemDescriptor));
        }

        [TestMethod()]
        public void TestIsProtonInConjugatedPiSystemDescriptor()
        {
            IAtomicDescriptor descriptor = new IsProtonInConjugatedPiSystemDescriptor();
            descriptor.Parameters = new object[] { true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CNC=CC=C");
            AddExplicitHydrogens(mol);
            Assert.IsTrue(((Result<bool>)descriptor.Calculate(mol.Atoms[13], mol).Value).Value);
        }
    }
}
