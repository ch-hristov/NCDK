



/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Numerics;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="PDBAtom"/>.
    /// </summary>
    [TestClass()]
    public class PDBAtomTest 
        : AbstractPDBAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new PDBAtom(ChemicalElement.R);
        }

        [TestMethod()]
        public void TestPDBAtom_IElement()
        {
            var element = ChemicalElement.R;
            IAtom a = new PDBAtom(element);
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public void TestPDBAtom_String()
        {
            IPDBAtom a = new PDBAtom("C");
            Assert.AreEqual("C", a.Symbol);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPDBAtom_String_Point3d()
        {
            Vector3 point3d = new Vector3(1.0, 2.0, 3.0);

            IPDBAtom a = new PDBAtom("C", point3d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point3d, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="PDBAtom"/>.
    /// </summary>
    [TestClass()]
    public class PDBAtomTest 
        : AbstractPDBAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new PDBAtom(ChemicalElement.R);
        }

        [TestMethod()]
        public void TestPDBAtom_IElement()
        {
            var element = ChemicalElement.R;
            IAtom a = new PDBAtom(element);
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public void TestPDBAtom_String()
        {
            IPDBAtom a = new PDBAtom("C");
            Assert.AreEqual("C", a.Symbol);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPDBAtom_String_Point3d()
        {
            Vector3 point3d = new Vector3(1.0, 2.0, 3.0);

            IPDBAtom a = new PDBAtom("C", point3d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point3d, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

 
                // Overwrite default methods: no notifications are expected!

        [TestMethod()]
        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetFlag()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlag(NewChemObject());
        }

        [TestMethod()]
        public void TestNotifyChanged_SetFlags()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlags(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestNotifyChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestStateChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestClone_ChemObjectListeners()
        {
            ChemObjectTestHelper.TestClone_ChemObjectListeners(NewChemObject());
        }

        [TestMethod()]
        public override void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestGetListenerCount()
        {
            ChemObjectTestHelper.TestGetListenerCount(NewChemObject());
        }

        [TestMethod()]
        public override void TestRemoveListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestRemoveListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestSetNotification_true()
        {
            ChemObjectTestHelper.TestSetNotification_true(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

    }
}
