#region .NET Disclaimer/Info

//===============================================================================
//
// gOODiDEA, uland.com
//===============================================================================
//
// $Header :		$  
// $Author :		$
// $Date   :		$
// $Revision:		$
// $History:		$  
//  
//===============================================================================

#endregion

#region Java

/* NeuQuant Neural-Net Quantization Algorithm
 * ------------------------------------------
 *
 * Copyright (c) 1994 Anthony Dekker
 *
 * NEUQUANT Neural-Net quantization algorithm by Anthony Dekker, 1994.
 * See "Kohonen neural networks for optimal colour quantization"
 * in "Network: Computation in Neural Systems" Vol. 5 (1994) pp 351-367.
 * for a discussion of the algorithm.
 *
 * Any party obtaining a copy of these files from the author, directly or
 * indirectly, is granted, free of charge, a full and unrestricted irrevocable,
 * world-wide, paid up, royalty-free, nonexclusive right and license to deal
 * in this software and documentation files (the "Software"), including without
 * limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons who receive
 * copies from any such party to do so, with the only requirement being
 * that this copyright notice remain intact.
 */

// Ported to Java 12/00 K Weiner

#endregion

namespace T2G.Components
{
    public class NeuQuant
    {
        private const int NetSize = 256; /* number of colours used */

        /* four primes near 500 - assume no image has a length so large */
        /* that it is divisible by all four primes */
        private const int Prime1 = 499;
        private const int Prime2 = 491;
        private const int Prime3 = 487;
        private const int Prime4 = 503;

        private const int MinPictureBytes = (3 * Prime4);

        /* minimum size for input image */
        /* Program Skeleton
           ----------------
           [select samplefac in range 1..30]
           [read image from input file]
           pic = (unsigned char*) malloc(3*width*height);
           initnet(pic,3*width*height,samplefac);
           learn();
           unbiasnet();
           [write output image header, using writecolourmap(f)]
           inxbuild();
           write output image using inxsearch(b,g,r)      */

        /* Network Definitions
           ------------------- */
        private const int MaxNetPos = (NetSize - 1);
        private const int NetBiasShift = 4; /* bias for colour values */
        private const int NCycles = 100; /* no. of learning cycles */

        /* defs for freq and bias */
        private const int IntBiasShift = 16; /* bias for fractions */
        private const int IntBias = 1 << IntBiasShift;
        private const int GammaShift = 10; /* gamma = 1024 */
        private const int BetaShift = 10;
        private const int Beta = (IntBias >> BetaShift); /* beta = 1/1024 */

        private const int BetaGamma = IntBias << (GammaShift - BetaShift);

        /* defs for decreasing radius factor */
        private const int InitRad = NetSize >> 3; /* for 256 cols, radius starts */
        private const int RadiusBiasShift = 6; /* at 32.0 biased by 6 bits */
        private const int RadiusBias = 1 << RadiusBiasShift;
        private const int InitRadius = (InitRad * RadiusBias); /* and decreases by a */
        private const int RadiusDec = 30; /* factor of 1/30 each cycle */

        /* defs for decreasing alpha factor */
        private const int AlphaBiasShift = 10; /* alpha starts at 1.0 */
        private const int InitAlpha = 1 << AlphaBiasShift;

        private int _alphaDec; /* biased by 10 bits */

        /* radbias and alpharadbias used for radpower calculation */
        private const int RadBiasShift = 8;
        private const int RadBias = 1 << RadBiasShift;
        private const int AlphaRadBShift = AlphaBiasShift + RadBiasShift;
        private const int AlphaRadBias = 1 << AlphaRadBShift;

        /* Types and Global Variables
        -------------------------- */

        private readonly byte[] _thePicture; /* the input image itself */
        private readonly int _lengthCount; /* lengthcount = H*W*3 */

        private int _sampleFac; /* sampling factor 1..30 */

        //   typedef int pixel[4];                /* BGRc */
        private readonly int[][] _network; /* the network itself - [netsize][4] */

        private readonly int[] _netIndex = new int[256];
        /* for network lookup - really 256 */

        private readonly int[] _bias = new int[NetSize];

        /* bias and freq arrays for learning */
        private readonly int[] _freq = new int[NetSize];

        private readonly int[] _radPower = new int[InitRad];
        /* radpower for precomputation */

        /* Initialise network in range (0,0,0) to (255,255,255) and set parameters
           ----------------------------------------------------------------------- */
        public NeuQuant(byte[] thePic, int len, int sample)
        {
            int i;
            int[] p;

            _thePicture = thePic;
            _lengthCount = len;
            _sampleFac = sample;

            _network = new int[NetSize][];
            for (i = 0; i < NetSize; i++)
            {
                _network[i] = new int[4];
                p = _network[i];
                p[0] = p[1] = p[2] = (i << (NetBiasShift + 8)) / NetSize;
                _freq[i] = IntBias / NetSize; /* 1/netsize */
                _bias[i] = 0;
            }
        }

        public byte[] ColorMap()
        {
            byte[] map = new byte[3 * NetSize];
            int[] index = new int[NetSize];
            for (int i = 0; i < NetSize; i++)
                index[_network[i][3]] = i;
            int k = 0;
            for (int i = 0; i < NetSize; i++)
            {
                int j = index[i];
                map[k++] = (byte) (_network[j][0]);
                map[k++] = (byte) (_network[j][1]);
                map[k++] = (byte) (_network[j][2]);
            }

            return map;
        }

        /* Insertion sort of network and building of netindex[0..255] (to do after unbias)
           ------------------------------------------------------------------------------- */
        public void Inxbuild()
        {
            int i, j, smallpos, smallval;
            int[] p;
            int[] q;
            int previouscol, startpos;

            previouscol = 0;
            startpos = 0;
            for (i = 0; i < NetSize; i++)
            {
                p = _network[i];
                smallpos = i;
                smallval = p[1]; /* index on g */
                /* find smallest in i..netsize-1 */
                for (j = i + 1; j < NetSize; j++)
                {
                    q = _network[j];
                    if (q[1] < smallval)
                    {
                        /* index on g */
                        smallpos = j;
                        smallval = q[1]; /* index on g */
                    }
                }

                q = _network[smallpos];
                /* swap p (i) and q (smallpos) entries */
                if (i != smallpos)
                {
                    j = q[0];
                    q[0] = p[0];
                    p[0] = j;
                    j = q[1];
                    q[1] = p[1];
                    p[1] = j;
                    j = q[2];
                    q[2] = p[2];
                    p[2] = j;
                    j = q[3];
                    q[3] = p[3];
                    p[3] = j;
                }

                /* smallval entry is now in position i */
                if (smallval != previouscol)
                {
                    _netIndex[previouscol] = (startpos + i) >> 1;
                    for (j = previouscol + 1; j < smallval; j++)
                        _netIndex[j] = i;
                    previouscol = smallval;
                    startpos = i;
                }
            }

            _netIndex[previouscol] = (startpos + MaxNetPos) >> 1;
            for (j = previouscol + 1; j < 256; j++)
                _netIndex[j] = MaxNetPos; /* really 256 */
        }

        /* Main Learning Loop
           ------------------ */
        public void Learn()
        {
            int i, j, b, g, r;
            int radius, rad, alpha, step, delta, samplepixels;
            byte[] p;
            int pix, lim;

            if (_lengthCount < MinPictureBytes)
                _sampleFac = 1;
            _alphaDec = 30 + ((_sampleFac - 1) / 3);
            p = _thePicture;
            pix = 0;
            lim = _lengthCount;
            samplepixels = _lengthCount / (3 * _sampleFac);
            delta = samplepixels / NCycles;
            alpha = InitAlpha;
            radius = InitRadius;

            rad = radius >> RadiusBiasShift;
            for (i = 0; i < rad; i++)
                _radPower[i] =
                    alpha * (((rad * rad - i * i) * RadBias) / (rad * rad));

            //fprintf(stderr,"beginning 1D learning: initial radius=%d\n", rad);

            if (_lengthCount < MinPictureBytes)
                step = 3;
            else if ((_lengthCount % Prime1) != 0)
                step = 3 * Prime1;
            else
            {
                if ((_lengthCount % Prime2) != 0)
                    step = 3 * Prime2;
                else
                {
                    if ((_lengthCount % Prime3) != 0)
                        step = 3 * Prime3;
                    else
                        step = 3 * Prime4;
                }
            }

            i = 0;
            while (i < samplepixels)
            {
                b = (p[pix + 0] & 0xff) << NetBiasShift;
                g = (p[pix + 1] & 0xff) << NetBiasShift;
                r = (p[pix + 2] & 0xff) << NetBiasShift;
                j = Contest(b, g, r);

                AlterSingle(alpha, j, b, g, r);
                if (rad != 0)
                    AlterNeigh(rad, j, b, g, r); /* alter neighbours */

                pix += step;
                if (pix >= lim)
                    pix -= _lengthCount;

                i++;
                if (delta == 0)
                    delta = 1;
                if (i % delta == 0)
                {
                    alpha -= alpha / _alphaDec;
                    radius -= radius / RadiusDec;
                    rad = radius >> RadiusBiasShift;
                    if (rad <= 1)
                        rad = 0;
                    for (j = 0; j < rad; j++)
                        _radPower[j] =
                            alpha * (((rad * rad - j * j) * RadBias) / (rad * rad));
                }
            }

            //fprintf(stderr,"finished 1D learning: readonly alpha=%f !\n",((float)alpha)/initalpha);
        }

        /* Search for BGR values 0..255 (after net is unbiased) and return colour index
           ---------------------------------------------------------------------------- */
        public int Map(int b, int g, int r)
        {
            int i, j, dist, a, bestd;
            int[] p;
            int best;

            bestd = 1000; /* biggest possible dist is 256*3 */
            best = -1;
            i = _netIndex[g]; /* index on g */
            j = i - 1; /* start at netindex[g] and work outwards */

            while ((i < NetSize) || (j >= 0))
            {
                if (i < NetSize)
                {
                    p = _network[i];
                    dist = p[1] - g; /* inx key */
                    if (dist >= bestd)
                        i = NetSize; /* stop iter */
                    else
                    {
                        i++;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }

                if (j >= 0)
                {
                    p = _network[j];
                    dist = g - p[1]; /* inx key - reverse dif */
                    if (dist >= bestd)
                        j = -1; /* stop iter */
                    else
                    {
                        j--;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }
            }

            return (best);
        }

        public byte[] Process()
        {
            Learn();
            UnBiasNet();
            Inxbuild();
            return ColorMap();
        }

        /* Unbias network to give byte values 0..255 and record position i to prepare for sort
           ----------------------------------------------------------------------------------- */
        private void UnBiasNet()
        {
            int i;

            for (i = 0; i < NetSize; i++)
            {
                _network[i][0] >>= NetBiasShift;
                _network[i][1] >>= NetBiasShift;
                _network[i][2] >>= NetBiasShift;
                _network[i][3] = i; /* record colour no */
            }
        }

        /* Move adjacent neurons by precomputed alpha*(1-((i-j)^2/[r]^2)) in radpower[|i-j|]
           --------------------------------------------------------------------------------- */
        private void AlterNeigh(int rad, int i, int b, int g, int r)
        {
            int j, k, lo, hi, a, m;
            int[] p;

            lo = i - rad;
            if (lo < -1)
                lo = -1;
            hi = i + rad;
            if (hi > NetSize)
                hi = NetSize;

            j = i + 1;
            k = i - 1;
            m = 1;
            while ((j < hi) || (k > lo))
            {
                a = _radPower[m++];
                if (j < hi)
                {
                    p = _network[j++];
                    try
                    {
                        p[0] -= (a * (p[0] - b)) / AlphaRadBias;
                        p[1] -= (a * (p[1] - g)) / AlphaRadBias;
                        p[2] -= (a * (p[2] - r)) / AlphaRadBias;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }  // prevents 1.3 misCompilation
                }

                if (k > lo)
                {
                    p = _network[k--];
                    try
                    {
                        p[0] -= (a * (p[0] - b)) / AlphaRadBias;
                        p[1] -= (a * (p[1] - g)) / AlphaRadBias;
                        p[2] -= (a * (p[2] - r)) / AlphaRadBias;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        /* Move neuron i towards biased (b,g,r) by factor alpha
           ---------------------------------------------------- */
        private void AlterSingle(int alpha, int i, int b, int g, int r)
        {
            /* alter hit neuron */
            int[] n = _network[i];
            n[0] -= (alpha * (n[0] - b)) / InitAlpha;
            n[1] -= (alpha * (n[1] - g)) / InitAlpha;
            n[2] -= (alpha * (n[2] - r)) / InitAlpha;
        }

        /* Search for biased BGR values
           ---------------------------- */
        private int Contest(int b, int g, int r)
        {
            /* finds closest neuron (min dist) and updates freq */
            /* finds best neuron (min dist-bias) and returns position */
            /* for frequently chosen neurons, freq[i] is high and bias[i] is negative */
            /* bias[i] = gamma*((1/netsize)-freq[i]) */

            int i, dist, a, biasdist, betafreq;
            int bestpos, bestbiaspos, bestd, bestbiasd;
            int[] n;

            bestd = ~(1 << 31);
            bestbiasd = bestd;
            bestpos = -1;
            bestbiaspos = bestpos;

            for (i = 0; i < NetSize; i++)
            {
                n = _network[i];
                dist = n[0] - b;
                if (dist < 0)
                    dist = -dist;
                a = n[1] - g;
                if (a < 0)
                    a = -a;
                dist += a;
                a = n[2] - r;
                if (a < 0)
                    a = -a;
                dist += a;
                if (dist < bestd)
                {
                    bestd = dist;
                    bestpos = i;
                }

                biasdist = dist - ((_bias[i]) >> (IntBiasShift - NetBiasShift));
                if (biasdist < bestbiasd)
                {
                    bestbiasd = biasdist;
                    bestbiaspos = i;
                }

                betafreq = (_freq[i] >> BetaShift);
                _freq[i] -= betafreq;
                _bias[i] += (betafreq << GammaShift);
            }

            _freq[bestpos] += Beta;
            _bias[bestpos] -= BetaGamma;
            return (bestbiaspos);
        }
    }
}