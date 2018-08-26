/*
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.SMSD.Algorithms.Matchers;
using NCDK.SMSD.Algorithms.VFLib.Builder;
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// Interface for the Node (atomss) in graph.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public interface INode
    {
        /// <summary>
        /// Returns Neighbors count.
        /// <returns>Neighbors count.</returns>
        /// </summary>
        int CountNeighbors();

        /// <summary>
        /// Returns neighbors.
        /// <returns>Iterable INode.</returns>
        /// </summary>
        IEnumerable<INode> Neighbors();

        /// <summary>
        /// Returns Query Atom.
        /// <returns>Query Atom.</returns>
        /// </summary>
        IVFAtomMatcher AtomMatcher { get; }

        /// <summary>
        /// Returns List of Edges.
        /// <returns>edges.</returns>
        /// </summary>
        IReadOnlyList<IEdge> GetEdges();

        /// <summary>
        /// Adds edge to the edge list.
        /// <param name="edge">add an edge.</param>
        /// </summary>
        void AddEdge(EdgeBuilder edge);

        /// <summary>
        /// Adds neighbor to the Neighbors List.
        /// <param name="node">add a node.</param>
        /// </summary>
        void AddNeighbor(NodeBuilder node);
    }
}
