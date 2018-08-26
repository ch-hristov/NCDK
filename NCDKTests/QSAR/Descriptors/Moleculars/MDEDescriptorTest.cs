using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.IO;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs a test for the MDEDescriptor.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class MDEDescriptorTest : MolecularDescriptorTest
    {
        public MDEDescriptorTest()
        {
            SetDescriptor(typeof(MDEDescriptor));
        }

        [TestMethod()]
        public void TestMDE1()
        {
            string filename = "NCDK.Data.MDL.mdeotest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            ArrayResult<double> result = (ArrayResult<double>)Descriptor.Calculate(ac).Value;

            Assert.AreEqual(0.0000, result[MDEDescriptor.MDEO11], 0.0001);
            Assert.AreEqual(1.1547, result[MDEDescriptor.MDEO12], 0.0001);
            Assert.AreEqual(2.9416, result[MDEDescriptor.MDEO22], 0.0001);
        }
    }
}
