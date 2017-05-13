/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Tools;
using System;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// XML version of the PDB format.
    /// See <see href="http://pdbml.rcsb.org/">here</see>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    public class PDBMLFormat : AbstractResourceFormat, IChemFormat
    {
        private static IResourceFormat myself = null;

        public PDBMLFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new PDBMLFormat();
                return myself;
            }
        }

        public override string FormatName => "Protein Data Bank Markup Language (PDBML)";
        public override string MIMEType => null;
        public override string PreferredNameExtension => null;
        public override string[] NameExtensions => Array.Empty<string>();
        public string ReaderClassName => null;
        public string WriterClassName => null;
        public override bool IsXmlBased => true;
        public int SupportedDataFeatures => DataFeatures.None;
        public int RequiredDataFeatures => DataFeatures.None;
    }
}