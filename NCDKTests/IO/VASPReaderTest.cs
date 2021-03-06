/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    // @cdk.module test-extra
    [TestClass()]
    public class VASPReaderTest 
        : SimpleChemObjectReaderTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        protected override string TestFile => "NCDK.Data.VASP.LiMoS2_optimisation_ISIF3.vasp";
        protected override Type ChemObjectIOToTestType => typeof(VASPReader);

        [TestMethod()]
        public void TestAccepts()
        {
            var reader = new VASPReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(IChemFile)));
        }

        [TestMethod()]
        public void TestReading()
        {
            var filename = "NCDK.Data.VASP.LiMoS2_optimisation_ISIF3.vasp";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new VASPReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            var sequence = chemFile[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(6, sequence.Count);
            var model = sequence[0];
            Assert.IsNotNull(model);
            var crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(16, crystal.Atoms.Count);
            var atom = crystal.Atoms[0];
            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.FractionalPoint3D);
        }
    }
}
