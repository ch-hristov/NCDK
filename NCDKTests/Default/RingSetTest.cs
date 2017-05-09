/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
    /// Checks the functionality of the RingSet class.
    /// </summary>
    // @cdk.module test-data
    // @see org.openscience.cdk.RingSet
    [TestClass()]
    public class RingSetTest : AbstractRingSetTest
    {
        public override IChemObject NewChemObject()
        {
            return new RingSet();
        }

        public override IRing NewContainerObject()
        {
            return new Ring();
        }

        [TestMethod()]
        public void TestRingSet()
        {
            IRingSet rs = new RingSet();
            Assert.IsNotNull(rs);
        }
    }
}
