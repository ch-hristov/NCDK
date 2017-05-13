/* Copyright (C) 2003-2007  The Jmol Development Team (v. 1.1.2.2)
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using NCDK.Common.Primitives;
using NCDK.Numerics;
using NCDK.Config;
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Read output files generated with the VASP software.
    /// </summary>
    // @cdk.module extra
    // @cdk.githash
    // @cdk.iooptions
    // @author  Fabian Dortu <Fabian.Dortu@wanadoo.be>
    public class VASPReader : DefaultChemObjectReader
    {
        // This variable is used to parse the input file
        protected string fieldVal;
        protected int repVal = 0;
        protected TextReader inputBuffer;

        IEnumerator<string> st = new List<string>().GetEnumerator();

        // VASP VARIABLES
        int natom = 1;
        int ntype = 1;
        double[] acell = new double[3];
        double[][] rprim = Arrays.CreateJagged<double>(3, 3);
        string info = "";
        string line;
        /// <summary>size is ntype. Contains the names of the atoms</summary>
        string[] anames;
        /// <summary>size is natom. Contain the atomic number</summary>
        int[] natom_type;
        /// <summary>"Direct" only so far</summary>
        string representation;

        /// <summary>
        /// Creates a new <see cref="VASPReader"/> instance.
        /// </summary>
        /// <param name="input">a <see cref="TextReader"/> value</param>
        public VASPReader(TextReader input)
        {
            this.inputBuffer = input;
        }

        public VASPReader(Stream input)
                : this(new StreamReader(input))
        { }

        public VASPReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => VASPFormat.Instance;

        public override void SetReader(TextReader input)
        {
            this.inputBuffer = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                IChemFile cf = (IChemFile)obj;
                try
                {
                    cf = ReadChemFile(cf);
                }
                catch (IOException exception)
                {
                    string error = "Input/Output error while reading from input: " + exception.Message;
                    Trace.TraceError(error);
                    Debug.WriteLine(exception);
                    throw new CDKException(error, exception);
                }
                return (T)cf;
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile.");
            }
        }

        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence seq = ReadChemSequence(file.Builder.CreateChemSequence());
            file.Add(seq);
            return file;
        }

        private IChemSequence ReadChemSequence(IChemSequence sequence)
        {
            IChemModel chemModel = sequence.Builder.CreateChemModel();
            ICrystal crystal = null;

            string buf = inputBuffer.ReadToEnd();
            inputBuffer = new StringReader(buf);

            // Get the info line (first token of the first line)
            info = NextVASPToken(false);
            //Debug.WriteLine(info);
            inputBuffer = new StringReader(buf);

            // Get the number of different atom "NCLASS=X"
            NextVASPTokenFollowing("NCLASS");
            ntype = int.Parse(fieldVal);
            //Debug.WriteLine("NCLASS= " + ntype);
            inputBuffer = new StringReader(buf);

            // Get the different atom names
            anames = new string[ntype];

            NextVASPTokenFollowing("ATOM");
            for (int i = 0; i < ntype; i++)
            {
                anames[i] = fieldVal;
                NextVASPToken(false);
            }

            // Get the number of atom of each type
            int[] natom_type = new int[ntype];
            natom = 0;
            for (int i = 0; i < ntype; i++)
            {
                natom_type[i] = int.Parse(fieldVal);
                NextVASPToken(false);
                natom = natom + natom_type[i];
            }

            // Get the representation type of the primitive vectors
            // only "Direct" is recognize now.
            representation = fieldVal;
            if (representation.Equals("Direct"))
            {
                Trace.TraceInformation("Direct representation");
                // DO NOTHING
            }
            else
            {
                throw new CDKException("This VASP file is not supported. Please contact the Jmol developpers");
            }

            while (NextVASPToken(false) != null)
            {
                Debug.WriteLine("New crystal started...");

                crystal = sequence.Builder.CreateCrystal();
                chemModel = sequence.Builder.CreateChemModel();

                // Get acell
                for (int i = 0; i < 3; i++)
                {
                    // TODO: supoort FORTRA format
                    acell[i] = double.Parse(fieldVal); //all the same FIX?
                }

                // Get primitive vectors
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        NextVASPToken(false);
                        // TODO: supoort FORTRA format
                        rprim[i][j] = double.Parse(fieldVal);
                    }

                // Get atomic position
                int[] atomType = new int[natom];
                double[][] xred = Arrays.CreateJagged<double>(natom, 3);
                int atomIndex = 0;

                for (int i = 0; i < ntype; i++)
                {
                    for (int j = 0; j < natom_type[i]; j++)
                    {
                        try
                        {
                            atomType[atomIndex] = Isotopes.Instance.GetElement(anames[i]).AtomicNumber.Value;
                        }
                        catch (Exception exception)
                        {
                            throw new CDKException("Could not determine atomic number!", exception);
                        }
                        Debug.WriteLine("aname: " + anames[i]);
                        Debug.WriteLine("atomType: " + atomType[atomIndex]);

                        NextVASPToken(false);
                        xred[atomIndex][0] = double.Parse(fieldVal);
                        NextVASPToken(false);
                        xred[atomIndex][1] = double.Parse(fieldVal);
                        NextVASPToken(false);
                        xred[atomIndex][2] = double.Parse(fieldVal);

                        atomIndex = atomIndex + 1;
                        // FIXME: store atom
                    }
                }

                crystal.A = new Vector3(rprim[0][0] * acell[0], rprim[0][1] * acell[0], rprim[0][2] * acell[0]);
                crystal.B = new Vector3(rprim[1][0] * acell[1], rprim[1][1] * acell[1], rprim[1][2] * acell[1]);
                crystal.C = new Vector3(rprim[2][0] * acell[2], rprim[2][1] * acell[2], rprim[2][2] * acell[2]);
                for (int i = 0; i < atomType.Length; i++)
                {
                    string symbol = "Du";
                    try
                    {
                        symbol = Isotopes.Instance.GetElement(atomType[i]).Symbol;
                    }
                    catch (Exception exception)
                    {
                        throw new CDKException("Could not determine element symbol!", exception);
                    }
                    IAtom atom = sequence.Builder.CreateAtom(symbol);
                    atom.AtomicNumber = atomType[i];
                    // convert fractional to cartesian
                    double[] frac = new double[3];
                    frac[0] = xred[i][0];
                    frac[1] = xred[i][1];
                    frac[2] = xred[i][2];
                    atom.FractionalPoint3D = new Vector3(frac[0], frac[1], frac[2]);
                    crystal.Atoms.Add(atom);
                }
                crystal.SetProperty(CDKPropertyName.Remark, info);
                chemModel.Crystal = crystal;

                Trace.TraceInformation("New Frame set!");

                sequence.Add(chemModel);

            } //end while

            return sequence;
        }

        /// <summary>
       /// Find the next token of an VASP file.
       /// ABINIT tokens are words separated by space(s). Characters
       /// following a "#" are ignored till the end of the line.
       ///
       /// <returns>a <see cref="string"/> value</returns>
       /// <exception cref="IOException">if an error occurs</exception>
       /// </summary>
        public string NextVASPToken(bool newLine)
        {
            string line = newLine ? null : "dummy";

            if (newLine)
            { // We ignore the end of the line and go to the following line
                if (true)
                {
                    line = inputBuffer.ReadLine();
                    if (line != null)
                        st = Strings.Tokenize(line, ' ', '=', '\t').GetEnumerator();
                }
            }

            while (!st.MoveNext() && (line = inputBuffer.ReadLine()) != null)
            {
                st = Strings.Tokenize(line, ' ', '=', '\t').GetEnumerator();
            }
            if (line != null)
            {
                fieldVal = st.Current;
                if (fieldVal.StartsWith("#"))
                {
                    NextVASPToken(true);
                }
            }
            else
            {
                fieldVal = null;
            }
            return this.fieldVal;
        } //end NextVASPToken(bool newLine)

        /// <summary>
        /// Find the next token of a VASP file beginning
        /// with the *next* line.
        /// </summary>
        public string NextVASPTokenFollowing(string str)
        {
            int index;
            string line;
            while ((line = inputBuffer.ReadLine()) != null)
            {
                index = line.IndexOf(str);
                if (index > 0)
                {
                    index = index + str.Length;
                    line = line.Substring(index);
                    st = Strings.Tokenize(line, ' ', '=', '\t').GetEnumerator();
                    while (!st.MoveNext() && (line = inputBuffer.ReadLine()) != null)
                    {
                        st = Strings.Tokenize(line, ' ', '=', '\t').GetEnumerator();
                    }
                    if (line != null)
                    {
                        fieldVal = st.Current;
                    }
                    else
                    {
                        fieldVal = null;
                    }
                    break;
                }
            }
            return fieldVal;
        } //end NextVASPTokenFollowing(string string)

        public override void Close()
        {
            inputBuffer.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
