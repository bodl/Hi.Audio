﻿/*
 * Copyright @ 2015 Atlassian Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * WARNING: The use of G.729 may require a license fee and/or royalty fee in
 * some countries and is licensed by
 * <a href="http://www.sipro.com">SIPRO Lab Telecom</a>.
 */

/**
 * Post-processing of output speech.
 * 2nd order high pass filter with cut off frequency at 100 Hz.
 * Designed with SPPACK efi command -40 dB att, 0.25 ri.
 *
 * Algorithm:
 * <pre>
 *  y[i] = b[0]*x[i] + b[1]*x[i-1] + b[2]*x[i-2]
 *                   + a[1]*y[i-1] + a[2]*y[i-2];
 *
 *     b[3] = {0.93980581E+00, -0.18795834E+01,  0.93980581E+00};
 *     a[3] = {0.10000000E+01, +0.19330735E+01, -0.93589199E+00};
 * </pre>
 *
 * @author Lubomir Marinov (translation of ITU-T C source code to Java)
 */
namespace Hi.Audio.Ref.GroovyCodecs.G729.Codec
{
    internal class PostPro
    {

        /* ITU-T G.729 Software Package Release 2 (November 2006) */
        /*
   ITU-T G.729 Annex C - Reference C code for floating point
                         implementation of G.729
                         Version 1.01 of 15.September.98
*/

        /*
----------------------------------------------------------------------
                    COPYRIGHT NOTICE
----------------------------------------------------------------------
   ITU-T G.729 Annex C ANSI C source code
   Copyright (C) 1998, AT&T, France Telecom, NTT, University of
   Sherbrooke.  All rights reserved.

----------------------------------------------------------------------
*/

        /*
 File : POST_PRO.C
 Used for the floating point version of both
 G.729 main body and G.729A
*/

        /**
 * High-pass fir memory
 */
        private float x0, x1;

        /**
 * High-pass iir memory
 */
        private float y1, y2;

        /**
 * Init Post Process.
 */

        public void init_post_process()
        {
            x0 = x1 = 0.0f;
            y2 = y1 = 0.0f;
        }

        /**
 * Post Process
 *
 * @param signal        (i/o)  : signal
 * @param lg            (i)    : lenght of signal
 */

        public void post_process(
            float[] signal,
            int lg
        )
        {
            var a100 = TabLd8k.a100;
            var b100 = TabLd8k.b100;

            int i;
            float x2;
            float y0;

            for (i = 0; i < lg; i++)
            {
                x2 = x1;
                x1 = x0;
                x0 = signal[i];

                y0 = y1 * a100[1] + y2 * a100[2] + x0 * b100[0] + x1 * b100[1] + x2 * b100[2];

                signal[i] = y0;
                y2 = y1;
                y1 = y0;
            }
        }
    }
}