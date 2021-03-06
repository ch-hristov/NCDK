using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Formula
{
    /// <summary>
    /// Class testing the IsotopeContainer class.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class IsotopeContainerTest : CDKTestCase
    {
        private static IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        ///  Constructor for the IsotopeContainerTest object.
        /// </summary>
        public IsotopeContainerTest()
                : base()
        { }
        [TestMethod()]
        public void TestIsotopeContainer()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            Assert.IsNotNull(isoC);
        }

        [TestMethod()]
        public void TestIsotopeContainer_IMolecularFormula_Double()
        {
            IMolecularFormula formula = builder.NewMolecularFormula();
            double intensity = 130.00;
            IsotopeContainer isoC = new IsotopeContainer(formula, intensity);

            Assert.IsNotNull(isoC);
            Assert.AreEqual(formula, isoC.Formula);
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        [TestMethod()]
        public void TestIsotopeContainer_Double_double()
        {
            double mass = 130.00;
            double intensity = 500000.00;
            IsotopeContainer isoC = new IsotopeContainer(mass, intensity);

            Assert.IsNotNull(isoC);
            Assert.AreEqual(mass, isoC.Mass, 0.001);
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        [TestMethod()]
        public void TestSetFormula_IMolecularFormula()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.NewMolecularFormula();
            isoC.Formula = formula;
            Assert.IsNotNull(isoC);
        }

        [TestMethod()]
        public void TestSetMass_Double()
        {
            IsotopeContainer isoC = new IsotopeContainer { Mass = 130.00 };
            Assert.IsNotNull(isoC);
        }

        [TestMethod()]
        public void TestSetIntensity_Double()
        {
            IsotopeContainer isoC = new IsotopeContainer { Intensity = 5000000.0 };
            Assert.IsNotNull(isoC);
        }

        [TestMethod()]
        public void TestGetFormula()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.NewMolecularFormula();
            isoC.Formula = formula;
            Assert.AreEqual(formula, isoC.Formula);
        }

        [TestMethod()]
        public void TestGetMass()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            double mass = 130.00;
            isoC.Mass = mass;
            Assert.AreEqual(mass, isoC.Mass, 0.001);

        }

        [TestMethod()]
        public void TestGetIntensity()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            double intensity = 130.00;
            isoC.Intensity = intensity;
            Assert.AreEqual(intensity, isoC.Intensity, 0.001);
        }

        [TestMethod()]
        public void TestClone()
        {
            IsotopeContainer isoC = new IsotopeContainer();
            IMolecularFormula formula = builder.NewMolecularFormula();
            isoC.Formula = formula;
            double mass = 130.00;
            isoC.Mass = mass;
            double intensity = 130.00;
            isoC.Intensity = intensity;

            IsotopeContainer clone = (IsotopeContainer)isoC.Clone();
            Assert.AreEqual(mass, clone.Mass, 0.001);
            Assert.AreEqual(intensity, clone.Intensity, 0.001);
            Assert.AreEqual(formula, clone.Formula);
        }
    }
}
