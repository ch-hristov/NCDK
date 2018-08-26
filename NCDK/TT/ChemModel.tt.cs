



// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
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

using System;
using System.Text;

namespace NCDK.Default
{
    /// <summary>
    /// An object containing multiple MoleculeSet and
    /// the other lower level concepts like rings, sequences,
    /// fragments, etc.
    /// </summary>
    // @cdk.githash 
    [Serializable]
    public class ChemModel
        : ChemObject, IChemModel, IChemObjectListener, ICloneable
    {
        /// <summary>
        ///  A molecule set.
        /// </summary>
        private IChemObjectSet<IAtomContainer> setOfMolecules = null;

        /// <summary>
        ///  A reaction set.
        /// </summary>
        private IReactionSet setOfReactions = null;
        private IRingSet ringSet = null;

        /// <summary>
        ///  A Crystal.
        /// </summary>
        private ICrystal crystal = null;

        /// <summary>
        ///  Constructs an new <see cref="ChemModel"/> with a null set of molecules.
        /// </summary>
        public ChemModel()
            : base()
        {
        }

        /// <summary>
        /// The molecule set of this <see cref="ChemModel"/>.
        /// </summary>
        public virtual IChemObjectSet<IAtomContainer> MoleculeSet
        {
            get { return setOfMolecules; }

            set
            {
 
                if (setOfMolecules != null)
                    setOfMolecules.Listeners.Remove(this);
                setOfMolecules = value;
 
                if (setOfMolecules != null)
                    setOfMolecules.Listeners.Add(this);
                NotifyChanged(); 
            }
        }

        /// <summary>
        /// The ring set of this <see cref="ChemModel"/>.
        /// </summary>
        public IRingSet RingSet
        {
            get { return ringSet; }

            set
            {
 
                if (ringSet != null)
                    ringSet.Listeners.Remove(this);
                ringSet = value;
 
                if (ringSet != null)
                    ringSet.Listeners.Add(this);
                NotifyChanged();
            }
        }

        /// <summary>
        /// The crystal contained in this <see cref="ChemModel"/>.
        /// </summary>
        public ICrystal Crystal
        {
            get { return crystal; }

            set
            {
 
                if (crystal != null)
                    crystal.Listeners.Remove(this);
                crystal = value;
 
                if (crystal != null)
                    crystal.Listeners.Add(this);
                NotifyChanged();
            }
        }

        /// <summary>
        /// The reaction set contained in this <see cref="ChemModel"/>.
        /// </summary>
        public IReactionSet ReactionSet
        {
            get { return setOfReactions; }

            set
            {
 
                if (setOfReactions != null)
                    setOfReactions.Listeners.Remove(this);
                setOfReactions = value;
 
                if (setOfReactions != null)
                    setOfReactions.Listeners.Add(this);
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemModel(");
            sb.Append(GetHashCode());
            if (MoleculeSet != null)
            {
                sb.Append(", ");
                sb.Append(MoleculeSet.ToString());
            }
            if (Crystal != null)
            {
                sb.Append(", ");
                sb.Append(Crystal.ToString());
            }
            if (ReactionSet != null)
            {
                sb.Append(", ");
                sb.Append(ReactionSet.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            ChemModel clone = (ChemModel)base.Clone(map);
            clone.setOfMolecules = (IChemObjectSet<IAtomContainer>)setOfMolecules?.Clone(map);
            clone.setOfReactions = (IReactionSet)setOfReactions?.Clone(map);
            clone.ringSet = (IRingSet)ringSet?.Clone(map);
            clone.crystal = (ICrystal)crystal?.Clone(map);
            return clone;
        }

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        /// </summary>
        /// <param name="evt">A change event pointing to the source of the change</param>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }

        /// <inheritdoc/>
        public bool IsEmpty()
            => (setOfMolecules == null || setOfMolecules.IsEmpty())
            && (setOfReactions == null || setOfReactions.IsEmpty())
            && (ringSet == null || ringSet.IsEmpty())
            && (crystal == null || crystal.IsEmpty());
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// An object containing multiple MoleculeSet and
    /// the other lower level concepts like rings, sequences,
    /// fragments, etc.
    /// </summary>
    // @cdk.githash 
    [Serializable]
    public class ChemModel
        : ChemObject, IChemModel, IChemObjectListener, ICloneable
    {
        /// <summary>
        ///  A molecule set.
        /// </summary>
        private IChemObjectSet<IAtomContainer> setOfMolecules = null;

        /// <summary>
        ///  A reaction set.
        /// </summary>
        private IReactionSet setOfReactions = null;
        private IRingSet ringSet = null;

        /// <summary>
        ///  A Crystal.
        /// </summary>
        private ICrystal crystal = null;

        /// <summary>
        ///  Constructs an new <see cref="ChemModel"/> with a null set of molecules.
        /// </summary>
        public ChemModel()
            : base()
        {
        }

        /// <summary>
        /// The molecule set of this <see cref="ChemModel"/>.
        /// </summary>
        public virtual IChemObjectSet<IAtomContainer> MoleculeSet
        {
            get { return setOfMolecules; }

            set
            {
                setOfMolecules = value;
            }
        }

        /// <summary>
        /// The ring set of this <see cref="ChemModel"/>.
        /// </summary>
        public IRingSet RingSet
        {
            get { return ringSet; }

            set
            {
                ringSet = value;
            }
        }

        /// <summary>
        /// The crystal contained in this <see cref="ChemModel"/>.
        /// </summary>
        public ICrystal Crystal
        {
            get { return crystal; }

            set
            {
                crystal = value;
            }
        }

        /// <summary>
        /// The reaction set contained in this <see cref="ChemModel"/>.
        /// </summary>
        public IReactionSet ReactionSet
        {
            get { return setOfReactions; }

            set
            {
                setOfReactions = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ChemModel(");
            sb.Append(GetHashCode());
            if (MoleculeSet != null)
            {
                sb.Append(", ");
                sb.Append(MoleculeSet.ToString());
            }
            if (Crystal != null)
            {
                sb.Append(", ");
                sb.Append(Crystal.ToString());
            }
            if (ReactionSet != null)
            {
                sb.Append(", ");
                sb.Append(ReactionSet.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            ChemModel clone = (ChemModel)base.Clone(map);
            clone.setOfMolecules = (IChemObjectSet<IAtomContainer>)setOfMolecules?.Clone(map);
            clone.setOfReactions = (IReactionSet)setOfReactions?.Clone(map);
            clone.ringSet = (IRingSet)ringSet?.Clone(map);
            clone.crystal = (ICrystal)crystal?.Clone(map);
            return clone;
        }

        /// <summary>
        ///  Called by objects to which this object has
        ///  registered as a listener.
        /// </summary>
        /// <param name="evt">A change event pointing to the source of the change</param>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }

        /// <inheritdoc/>
        public bool IsEmpty()
            => (setOfMolecules == null || setOfMolecules.IsEmpty())
            && (setOfReactions == null || setOfReactions.IsEmpty())
            && (ringSet == null || ringSet.IsEmpty())
            && (crystal == null || crystal.IsEmpty());
    }
}
