﻿//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

/*
 * decode_i396.c: Mpeg Layer-1,2,3 audio decoder
 *
 * Copyright (C) 1999-2010 The L.A.M.E. project
 *
 * Initially written by Michael Hipp, see also AUTHORS and README.
 *  
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 *
 *
 * Slighlty optimized for machines without autoincrement/decrement.
 * The performance is highly compiler dependend. Maybe
 * the decode.c version for 'normal' processor may be faster
 * even for Intel processors.
 */

/* $Id: Decode.java,v 1.10 2011/05/31 03:33:59 kenchis Exp $ */

namespace Hi.Audio.Ref.GroovyCodecs.Mp3.Mpg
{

    internal class Decode
    {

        internal interface Factory<T>
        {
            T create(float x);
        }

        private const int step = 2;

        private DCT64 dct64;

        private TabInit tab;

        internal virtual void setModules(TabInit t, DCT64 d)
        {
            tab = t;
            dct64 = d;
        }

        /* old WRITE_SAMPLE_CLIPPED */
        private int WRITE_SAMPLE_CLIPPED<T>(int samples, float sum, int clip, T[] @out, Factory<T> tFactory)
        {
            /* old WRITE_SAMPLE_CLIPPED */
            if (sum > 32767.0)
            {
                @out[samples] = tFactory.create(32767);
                clip++;
            }
            else if (sum < -32768.0)
            {
                @out[samples] = tFactory.create(-32768);
                clip++;
            }
            else
            {
                @out[samples] = tFactory.create((int)(sum > 0 ? sum + 0.5 : sum - 0.5));
            }

            return clip;
        }

        private void WRITE_SAMPLE_UNCLIPPED<T>(int samples, float sum, int clip, T[] @out, Factory<T> tFactory)
        {
            @out[samples] = tFactory.create(sum);
        }

        internal virtual int synth_1to1_mono<T>(
            MPGLib.mpstr_tag mp,
            float[] bandPtr,
            int bandPos,
            T[] @out,
            MPGLib.ProcessedBytes pnt,
            Factory<T> tFactory)
        {
            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

            var samples_tmp = new T[64];
            var tmp1 = 0;
            int i, ret;
            var pnt1 = new MPGLib.ProcessedBytes();

            ret = synth_1to1(mp, bandPtr, bandPos, 0, samples_tmp, pnt1, tFactory);
            var outPos = pnt.pb;

            for (i = 0; i < 32; i++)
            {
                @out[outPos++] = samples_tmp[tmp1];
                tmp1 += 2;
            }

            pnt.pb += 32;

            return ret;
        }

        internal virtual int synth_1to1_mono_unclipped<T>(
            MPGLib.mpstr_tag mp,
            float[] bandPtr,
            int bandPos,
            T[] @out,
            MPGLib.ProcessedBytes pnt,
            Factory<T> tFactory)
        {
            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

            var samples_tmp = new T[64];
            var tmp1 = 0;
            int i, ret;
            var pnt1 = new MPGLib.ProcessedBytes();

            ret = synth_1to1_unclipped(mp, bandPtr, bandPos, 0, samples_tmp, pnt1, tFactory);
            var outPos = pnt.pb;

            for (i = 0; i < 32; i++)
            {
                @out[outPos++] = samples_tmp[tmp1];
                tmp1 += 2;
            }

            pnt.pb += 32;

            return ret;
        }

        internal virtual int synth_1to1<T>(
            MPGLib.mpstr_tag mp,
            float[] bandPtr,
            int bandPos,
            int channel,
            T[] @out,
            MPGLib.ProcessedBytes pnt,
            Factory<T> tFactory)
        {
            int bo;
            var samples = pnt.pb;
            float[] b0;
            int b0Pos;
            float[][] buf;
            var clip = 0;
            int bo1;

            bo = mp.synth_bo;

            if (0 == channel)
            {
                bo--;
                bo &= 0xf;
                buf = mp.synth_buffs[0];
            }
            else
            {
                samples++;
                buf = mp.synth_buffs[1];
            }

            if ((bo & 0x1) != 0)
            {
                b0 = buf[0];
                b0Pos = 0;
                bo1 = bo;
                dct64.dct64(buf[1], (bo + 1) & 0xf, buf[0], bo, bandPtr, bandPos);
            }
            else
            {
                b0 = buf[1];
                b0Pos = 0;
                bo1 = bo + 1;
                dct64.dct64(buf[0], bo, buf[1], bo + 1, bandPtr, bandPos);
            }

            mp.synth_bo = bo;

            {
                int j;
                var window = 16 - bo1;

                for (j = 16; j != 0; j--, b0Pos += 0x10, window += 0x20, samples += step)
                {
                    float sum;
                    sum = tab.decwin[window + 0x0] * b0[b0Pos + 0x0];
                    sum -= tab.decwin[window + 0x1] * b0[b0Pos + 0x1];
                    sum += tab.decwin[window + 0x2] * b0[b0Pos + 0x2];
                    sum -= tab.decwin[window + 0x3] * b0[b0Pos + 0x3];
                    sum += tab.decwin[window + 0x4] * b0[b0Pos + 0x4];
                    sum -= tab.decwin[window + 0x5] * b0[b0Pos + 0x5];
                    sum += tab.decwin[window + 0x6] * b0[b0Pos + 0x6];
                    sum -= tab.decwin[window + 0x7] * b0[b0Pos + 0x7];
                    sum += tab.decwin[window + 0x8] * b0[b0Pos + 0x8];
                    sum -= tab.decwin[window + 0x9] * b0[b0Pos + 0x9];
                    sum += tab.decwin[window + 0xA] * b0[b0Pos + 0xA];
                    sum -= tab.decwin[window + 0xB] * b0[b0Pos + 0xB];
                    sum += tab.decwin[window + 0xC] * b0[b0Pos + 0xC];
                    sum -= tab.decwin[window + 0xD] * b0[b0Pos + 0xD];
                    sum += tab.decwin[window + 0xE] * b0[b0Pos + 0xE];
                    sum -= tab.decwin[window + 0xF] * b0[b0Pos + 0xF];
                    clip = WRITE_SAMPLE_CLIPPED(samples, sum, clip, @out, tFactory);
                }

                {
                    float sum;
                    sum = tab.decwin[window + 0x0] * b0[b0Pos + 0x0];
                    sum += tab.decwin[window + 0x2] * b0[b0Pos + 0x2];
                    sum += tab.decwin[window + 0x4] * b0[b0Pos + 0x4];
                    sum += tab.decwin[window + 0x6] * b0[b0Pos + 0x6];
                    sum += tab.decwin[window + 0x8] * b0[b0Pos + 0x8];
                    sum += tab.decwin[window + 0xA] * b0[b0Pos + 0xA];
                    sum += tab.decwin[window + 0xC] * b0[b0Pos + 0xC];
                    sum += tab.decwin[window + 0xE] * b0[b0Pos + 0xE];
                    clip = WRITE_SAMPLE_CLIPPED(samples, sum, clip, @out, tFactory);
                    b0Pos -= 0x10;
                    window -= 0x20;
                    samples += step;
                }
                window += bo1 << 1;

                for (j = 15; j != 0; j--, b0Pos -= 0x10, window -= 0x20, samples += step)
                {
                    float sum;
                    sum = -tab.decwin[window + -0x1] * b0[b0Pos + 0x0];
                    sum -= tab.decwin[window + -0x2] * b0[b0Pos + 0x1];
                    sum -= tab.decwin[window + -0x3] * b0[b0Pos + 0x2];
                    sum -= tab.decwin[window + -0x4] * b0[b0Pos + 0x3];
                    sum -= tab.decwin[window + -0x5] * b0[b0Pos + 0x4];
                    sum -= tab.decwin[window + -0x6] * b0[b0Pos + 0x5];
                    sum -= tab.decwin[window + -0x7] * b0[b0Pos + 0x6];
                    sum -= tab.decwin[window + -0x8] * b0[b0Pos + 0x7];
                    sum -= tab.decwin[window + -0x9] * b0[b0Pos + 0x8];
                    sum -= tab.decwin[window + -0xA] * b0[b0Pos + 0x9];
                    sum -= tab.decwin[window + -0xB] * b0[b0Pos + 0xA];
                    sum -= tab.decwin[window + -0xC] * b0[b0Pos + 0xB];
                    sum -= tab.decwin[window + -0xD] * b0[b0Pos + 0xC];
                    sum -= tab.decwin[window + -0xE] * b0[b0Pos + 0xD];
                    sum -= tab.decwin[window + -0xF] * b0[b0Pos + 0xE];
                    sum -= tab.decwin[window + -0x0] * b0[b0Pos + 0xF];

                    clip = WRITE_SAMPLE_CLIPPED(samples, sum, clip, @out, tFactory);
                }
            }
            pnt.pb += 64;

            return clip;
        }

        internal virtual int synth_1to1_unclipped<T>(
            MPGLib.mpstr_tag mp,
            float[] bandPtr,
            int bandPos,
            int channel,
            T[] @out,
            MPGLib.ProcessedBytes pnt,
            Factory<T> tFactory)
        {
            int bo;
            var samples = pnt.pb;
            float[] b0;
            int b0Pos;
            float[][] buf;
            var clip = 0;
            int bo1;

            bo = mp.synth_bo;

            if (0 == channel)
            {
                bo--;
                bo &= 0xf;
                buf = mp.synth_buffs[0];
            }
            else
            {
                samples++;
                buf = mp.synth_buffs[1];
            }

            if ((bo & 0x1) != 0)
            {
                b0 = buf[0];
                b0Pos = 0;
                bo1 = bo;
                dct64.dct64(buf[1], (bo + 1) & 0xf, buf[0], bo, bandPtr, bandPos);
            }
            else
            {
                b0 = buf[1];
                b0Pos = 0;
                bo1 = bo + 1;
                dct64.dct64(buf[0], bo, buf[1], bo + 1, bandPtr, bandPos);
            }

            mp.synth_bo = bo;

            {
                int j;
                var window = 16 - bo1;

                for (j = 16; j != 0; j--, b0Pos += 0x10, window += 0x20, samples += step)
                {
                    float sum;
                    sum = tab.decwin[window + 0x0] * b0[b0Pos + 0x0];
                    sum -= tab.decwin[window + 0x1] * b0[b0Pos + 0x1];
                    sum += tab.decwin[window + 0x2] * b0[b0Pos + 0x2];
                    sum -= tab.decwin[window + 0x3] * b0[b0Pos + 0x3];
                    sum += tab.decwin[window + 0x4] * b0[b0Pos + 0x4];
                    sum -= tab.decwin[window + 0x5] * b0[b0Pos + 0x5];
                    sum += tab.decwin[window + 0x6] * b0[b0Pos + 0x6];
                    sum -= tab.decwin[window + 0x7] * b0[b0Pos + 0x7];
                    sum += tab.decwin[window + 0x8] * b0[b0Pos + 0x8];
                    sum -= tab.decwin[window + 0x9] * b0[b0Pos + 0x9];
                    sum += tab.decwin[window + 0xA] * b0[b0Pos + 0xA];
                    sum -= tab.decwin[window + 0xB] * b0[b0Pos + 0xB];
                    sum += tab.decwin[window + 0xC] * b0[b0Pos + 0xC];
                    sum -= tab.decwin[window + 0xD] * b0[b0Pos + 0xD];
                    sum += tab.decwin[window + 0xE] * b0[b0Pos + 0xE];
                    sum -= tab.decwin[window + 0xF] * b0[b0Pos + 0xF];
                    WRITE_SAMPLE_UNCLIPPED(samples, sum, clip, @out, tFactory);
                }

                {
                    float sum;
                    sum = tab.decwin[window + 0x0] * b0[b0Pos + 0x0];
                    sum += tab.decwin[window + 0x2] * b0[b0Pos + 0x2];
                    sum += tab.decwin[window + 0x4] * b0[b0Pos + 0x4];
                    sum += tab.decwin[window + 0x6] * b0[b0Pos + 0x6];
                    sum += tab.decwin[window + 0x8] * b0[b0Pos + 0x8];
                    sum += tab.decwin[window + 0xA] * b0[b0Pos + 0xA];
                    sum += tab.decwin[window + 0xC] * b0[b0Pos + 0xC];
                    sum += tab.decwin[window + 0xE] * b0[b0Pos + 0xE];
                    WRITE_SAMPLE_UNCLIPPED(samples, sum, clip, @out, tFactory);
                    b0Pos -= 0x10;
                    window -= 0x20;
                    samples += step;
                }
                window += bo1 << 1;

                for (j = 15; j != 0; j--, b0Pos -= 0x10, window -= 0x20, samples += step)
                {
                    float sum;
                    sum = -tab.decwin[window + -0x1] * b0[b0Pos + 0x0];
                    sum -= tab.decwin[window + -0x2] * b0[b0Pos + 0x1];
                    sum -= tab.decwin[window + -0x3] * b0[b0Pos + 0x2];
                    sum -= tab.decwin[window + -0x4] * b0[b0Pos + 0x3];
                    sum -= tab.decwin[window + -0x5] * b0[b0Pos + 0x4];
                    sum -= tab.decwin[window + -0x6] * b0[b0Pos + 0x5];
                    sum -= tab.decwin[window + -0x7] * b0[b0Pos + 0x6];
                    sum -= tab.decwin[window + -0x8] * b0[b0Pos + 0x7];
                    sum -= tab.decwin[window + -0x9] * b0[b0Pos + 0x8];
                    sum -= tab.decwin[window + -0xA] * b0[b0Pos + 0x9];
                    sum -= tab.decwin[window + -0xB] * b0[b0Pos + 0xA];
                    sum -= tab.decwin[window + -0xC] * b0[b0Pos + 0xB];
                    sum -= tab.decwin[window + -0xD] * b0[b0Pos + 0xC];
                    sum -= tab.decwin[window + -0xE] * b0[b0Pos + 0xD];
                    sum -= tab.decwin[window + -0xF] * b0[b0Pos + 0xE];
                    sum -= tab.decwin[window + -0x0] * b0[b0Pos + 0xF];

                    WRITE_SAMPLE_UNCLIPPED(samples, sum, clip, @out, tFactory);
                }
            }
            pnt.pb += 64;

            return clip;
        }
    }

}