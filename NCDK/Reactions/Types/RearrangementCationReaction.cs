/* Copyright (C) 2008 Miguel Rojas <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Reactions.Types.Parameters;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// <para>IReactionProcess which participate in movement resonance.
    /// This reaction could be represented as [A+]-B=C => A=B-[c+]. Due to
    /// deficiency of charge of the atom A, the double bond is desplaced.</para>
    /// <para>Make sure that the molecule has the corresponend lone pair electrons
    /// for each atom. You can use the method: <see cref="Tools.LonePairElectronChecker"/></para>
    /// </summary>
    /// <seealso cref="Mechanisms.RearrangementChargeMechanism"/>
    // @author         Miguel Rojas
    // @cdk.created    2006-05-05
    // @cdk.module     reaction
    // @cdk.githash
    public partial class RearrangementCationReaction : AbstractRearrangementReaction
    {
        public RearrangementCationReaction() { }

        /// <inheritdoc/>
        public override ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#RearrangementCation", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            return base.Initiate(reactants, agents,
                null,
                (mol, atom) => atom.FormalCharge == 1,
                atom => (atom.FormalCharge ?? 0) == 0);
        }
    }
}
