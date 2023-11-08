using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedNoise {
    static ImprovedNoise() {
        p = new int[512];
        for ( int x = 0; x < 512; x++ ) {
            p[x] = permutation[x % 256];
        }
    }

    static public double noise( float x, float y, float z ) {
        int X = ( int )Mathf.Floor( x ) & 255,                  // FIND UNIT CUBE THAT
            Y = ( int )Mathf.Floor( y ) & 255,                  // CONTAINS POINT.
            Z = ( int )Mathf.Floor( z ) & 255;
        x -= Mathf.Floor( x );                                // FIND RELATIVE X,Y,Z
        y -= Mathf.Floor( y );                                // OF POINT IN CUBE.
        z -= Mathf.Floor( z );
        double u = fade( x ),                                // COMPUTE FADE CURVES
               v = fade( y ),                                // FOR EACH OF X,Y,Z.
               w = fade( z );
        int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z,      // HASH COORDINATES OF
            B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;      // THE 8 CUBE CORNERS,

        return lerp( w, lerp( v, lerp( u, grad( p[AA], x, y, z ),  // AND ADD
                                       grad( p[BA], x - 1, y, z ) ), // BLENDED
                               lerp( u, grad( p[AB], x, y - 1, z ),  // RESULTS
                                       grad( p[BB], x - 1, y - 1, z ) ) ),// FROM  8
                       lerp( v, lerp( u, grad( p[AA + 1], x, y, z - 1 ),  // CORNERS
                                       grad( p[BA + 1], x - 1, y, z - 1 ) ), // OF CUBE
                               lerp( u, grad( p[AB + 1], x, y - 1, z - 1 ),
                                       grad( p[BB + 1], x - 1, y - 1, z - 1 ) ) ) );
    }
    static double fade( double t ) { return t * t * t * ( t * ( t * 6 - 15 ) + 10 ); }
    static double lerp( double t, double a, double b ) { return a + t * ( b - a ); }
    static double grad( int hash, double x, double y, double z ) {
        int h = hash & 15;                      // CONVERT LO 4 BITS OF HASH CODE
        double u = h < 8 ? x : y,                 // INTO 12 GRADIENT DIRECTIONS.
               v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ( ( h & 1 ) == 0 ? u : -u ) + ( ( h & 2 ) == 0 ? v : -v );
    }
    static int[] permutation = { 151,160,137,91,90,15,
   131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
   190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
   88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
   77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
   102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
   135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
   5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
   223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
   129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
   251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
   49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
   138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
   };
    private static int[] p;
    //static { for (int i=0; i< 256 ; i++) p[256 + i] = p[i] = permutation[i]; }
}

public class Perlin  {

    bool start = true;
    int B = 256;
    int BM = 255;
    int N = 4096;
    int[] p;
    float[] g1;
    Vector2[] g2;//[B + B + 2][2];
    Vector3[] g3;//[B + B + 2][3];
    // Function to linearly interpolate between a0 and a1
    // Weight w should be in the range [0.0, 1.0]
    float lerp( float a0, float a1, float w ) {
        return ( 1.0f - w ) * a0 + w * a1;

        // as an alternative, this slightly faster equivalent formula can be used:
        // return a0 + w*(a1 - a0);
    }

    // Computes the dot product of the distance and gradient vectors.
    float dotGridGradient( int ix, int iy, float x, float y ) {

        // Precomputed (or otherwise) gradient vectors at each grid node
       // float Gradient[IYMAX][IXMAX] [2];

        // Compute the distance vector
        float dx = x - ( float )ix;
        float dy = y - ( float )iy;

        // Compute the dot-product
        return 0f;//(dx* Gradient[iy][ix][0] + dy* Gradient[iy][ix] [1]);
    }
    int random() {
        return Random.Range( 0, 4000000 );
    }

    void init( ) {
        int i, j, k;
        p = new int[B + B + 2];
        g1 = new float[B + B + 2];
        g2 = new Vector2[B + B + 2];
        g3 = new Vector3[B + B + 2];
        for ( i = 0; i < B; i++ ) {
            p[i] = i;

            g1[i] = ( float )( ( random() % ( B + B ) ) - B ) / B;

            g2[i] =new Vector2(  ((random() % ( B + B ) ) - B ) / B,(( random() % ( B + B ) ) - B ) / B);
            g2[i].Normalize();

            for ( j = 0; j < 3; j++ )
                g3[i][j] = ( float )( ( random() % ( B + B ) ) - B ) / B;
            g3[i].Normalize();
        }

        while (--i > 0) {
            k = p[i];
            p[i] = p[j = random() % B];
            p[j] = k;
        }

        for ( i = 0; i < B + 2; i++ ) {
            p[B + i] = p[i];
            g1[B + i] = g1[i];
            for ( j = 0; j < 2; j++ )
                g2[B + i][j] = g2[i][j];
            for ( j = 0; j < 3; j++ )
                g3[B + i][j] = g3[i][j];
        }
    }

    public float noise1( float arg ) {
        int bx0, bx1;
        float rx0, rx1, sx, t, u, v;
        float[] vec = new float[1];

        vec[0] = arg;
        if ( start ) {
            start = false;
            init();
        }

        t = vec[0] + N;
        bx0 = ( ( int )t ) & BM;
        bx1 = ( bx0 + 1 ) & BM;
        rx0 = t - ( int )t;
        rx1 = rx0 - 1.0f;
     //  setup( 0, bx0, bx1, rx0, rx1 );

        sx = s_curve( rx0 );

        u = rx0 * g1[p[bx0]];
        v = rx1 * g1[p[bx1]];

        return lerp( sx, u, v );
    }

    public float noise2( Vector2 vec) {
        int bx0, bx1, by0, by1, b00, b10, b01, b11;
        float rx0, rx1, ry0, ry1, sx, sy, a, b, t, u, v;
        Vector2 q;
        int i, j;

        if ( start ) {
            start = false;
            init();
        }

        //Setup(0, bx0,bx1, rx0,rx1);
       /*setup( i, b0, b1, r0, r1 )\
        t = vec[i] + N;\
        b0 = ( ( int )t ) & BM;\
        b1 = ( b0 + 1 ) & BM;\
        r0 = t - ( int )t;\
        r1 = r0 - 1.;  */

        t = vec[0] + N;
        bx0 = ( ( int )t ) & BM;
        bx1 = ( bx0 + 1 ) & BM;
        rx0 = t - ( int )t;
        rx1 = rx0 - 1.0f;

        //Setup(1, by0,by1, ry0,ry1);
        t = vec[1] + N;
        by0 = ( ( int )t ) & BM;
        by1 = ( by0 + 1 ) & BM;
        ry0 = t - ( int )t;
        ry1 = ry0 - 1.0f;

        i = p[bx0];
        j = p[bx1];

        b00 = p[i + by0];
        b10 = p[j + by0];
        b01 = p[i + by1];
        b11 = p[j + by1];

        sx = s_curve( rx0 );
        sy = s_curve( ry0 );

 

        q = g2[b00];
        u = at2( rx0, ry0, q );

        q = g2[b10]; 
        v = at2( rx1, ry0 ,q);
        a = lerp( sx, u, v );


        q = g2[b01];
        u = at2( rx0, ry1,q );

        q = g2[b11];
        v = at2( rx1, ry1,q );
        b = lerp( sx, u, v );

        return lerp( sy, a, b );
    }
    float at2(float rx,float ry, Vector2 q) {
        return ( rx * q[0] + ry * q[1] );
    }
    float s_curve( float t ) {
        return t* t *( 3.0f - 2.0f * t );
    }

    /*void setup(flo i, b0, b1, r0, r1) {
        t = vec[i] + N;\
    b0 = ( ( int )t ) & BM;\
    b1 = ( b0 + 1 ) & BM;\
    r0 = t - ( int )t;\
    r1 = r0 - 1.;
    }*/

    // Compute Perlin noise at coordinates x, y
 /*   float perlin( float x, float y ) {

        // Determine grid cell coordinates
        int x0 = (int)x;
        int x1 = x0 + 1;
        int y0 = (int) y ;
        int y1 = y0 + 1;

        // Determine interpolation weights
        // Could also use higher order polynomial/s-curve here
        float sx = x - ( float )x0;
        float sy = y - ( float )y0;

        // Interpolate between grid point gradients
        float n0, n1, ix0, ix1, value;
        n0 = dotGridGradient( x0, y0, x, y );
        n1 = dotGridGradient( x1, y0, x, y );
        ix0 = lerp( n0, n1, sx );
        n0 = dotGridGradient( x0, y1, x, y );
        n1 = dotGridGradient( x1, y1, x, y );
        ix1 = lerp( n0, n1, sx );
        value = lerp( ix0, ix1, sy );

        return value;
    }*/
}
