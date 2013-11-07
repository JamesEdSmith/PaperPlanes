package ca.smiths.games.paperplanes;

/**
 * Created by james.smith on 2013-11-04.
 */
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;

import javax.microedition.khronos.opengles.GL10;

/**
 * A vertex shaded paper.
 */
class Paper
{
    int mColors[];
    int one;
    int two;

    public Paper()
    {
        mColors = new int[16];
        one = 0x10000;
        two = 0x10000 * 2;
        int vertices[] = {
                -one, -one, -one,
                one, -one, -one,
                one,  one, -one,
                -one,  one, -one,
        };

        int colors[] = {
                one,    one,    one,  one,
                one,    one,    one,  one,
                one,  one,    one,  one,
                one,  one,    one,  one,
        };

        byte indices[] = {
                0, 1, 3,    3, 1, 2,
                0, 3, 1,    3, 2, 1,
        };

        // Buffers to be passed to gl*Pointer() functions
        // must be direct, i.e., they must be placed on the
        // native heap where the garbage collector cannot
        // move them.
        //
        // Buffers with multi-byte datatypes (e.g., short, int, float)
        // must have their byte order set to native order

        ByteBuffer vbb = ByteBuffer.allocateDirect(vertices.length*4);
        vbb.order(ByteOrder.nativeOrder());
        mVertexBuffer = vbb.asIntBuffer();
        mVertexBuffer.put(vertices);
        mVertexBuffer.position(0);

        ByteBuffer cbb = ByteBuffer.allocateDirect(colors.length*4);
        cbb.order(ByteOrder.nativeOrder());
        mColorBuffer = cbb.asIntBuffer();
        mColorBuffer.put(colors);
        mColorBuffer.position(0);

        mIndexBuffer = ByteBuffer.allocateDirect(indices.length);
        mIndexBuffer.put(indices);
        mIndexBuffer.position(0);
    }

    public void draw(GL10 gl)
    {
        gl.glVertexPointer(3, gl.GL_FIXED, 0, mVertexBuffer);
        gl.glColorPointer(4, gl.GL_FIXED, 0, mColorBuffer);
        gl.glDrawElements(gl.GL_TRIANGLES, 12, gl.GL_UNSIGNED_BYTE, mIndexBuffer);
    }

    public int[] cross(int[] x, int[] y, int[] z)
    {
        int[] normal = new int [3];
        normal[0] = (y[1] * z[2]) - (y[2] * z[1]);
        normal[1] = (z[1] * x[2]) - (z[2] * x[1]);
        normal[2] = (x[1] * y[2]) - (x[2] * y[1]);

        return normal;
    }

    public void setColors(int color)
    {
        for(int i = 0; i<16; i++)
        {
            if((i+1)%4 == 0)
                mColors[i] = 65536;
            else
                mColors[i] = color;
        }
        ByteBuffer cbb = ByteBuffer.allocateDirect(mColors.length*4);
        cbb.order(ByteOrder.nativeOrder());
        mColorBuffer = cbb.asIntBuffer();
        mColorBuffer.put(mColors);
        mColorBuffer.position(0);
    }

    private IntBuffer   mVertexBuffer;
    private IntBuffer   mColorBuffer;
    private ByteBuffer  mIndexBuffer;
}

