package ca.smiths.games.paperplanes;

/**
 * Created by james.smith on 2013-11-04.
 */
import com.badlogic.gdx.math.Vector3;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;
import java.nio.FloatBuffer;

import javax.microedition.khronos.opengles.GL10;

import java.util.List;

/**
 * A vertex shaded paper.
 */
class Paper
{
    int mColors[];
    int one;
    int two;
    public float position [] = {0,0,-2};
    public float rotation [] = {0,0,0};

    public List<Vector3> verts;
    private IntBuffer   fVertexBuffer;
    private IntBuffer   fColorBuffer;
    private ByteBuffer  fIndexBuffer;
    private FloatBuffer fNormalBuffer;
    private ByteBuffer  bIndexBuffer;
    private FloatBuffer bNormalBuffer;

    public Paper()
    {
        mColors = new int[16];
        one = 0x10000;
        two = 0x10000 * 2;
        int vertices[] = {
                -one, -one, 0,
                one, -one, 0,
                one,  one, 0,
                -one,  one, 0,
        };

        verts = new List<Vector3>();
        for(int i =0; i<12; i+3)
        {
            verts.add(new Vector3(vertices[i], vertices[i+1],vertices[i+2]));
        }


        float fNormals[] = {
            0,0,1,
            0,0,1,
            0,0,1,
            0,0,1,
        };

        float bNormals[] = {
                0,0,-1,
                0,0,-1,
                0,0,-1,
                0,0,-1,
        };

        int colors[] = {
                one,    one,    one,  one,
                one,    one,    one,  one,
                one,  one,    one,  one,
                one,  one,    one,  one,
        };

        byte fIndices[] = {
                0, 1, 2,    0, 2, 3,
        };

        byte bIndices[] = {
                1, 0, 3,    1, 3, 2,
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
        fVertexBuffer = vbb.asIntBuffer();
        fVertexBuffer.put(vertices);
        fVertexBuffer.position(0);

        ByteBuffer cbb = ByteBuffer.allocateDirect(colors.length*4);
        cbb.order(ByteOrder.nativeOrder());
        fColorBuffer = cbb.asIntBuffer();
        fColorBuffer.put(colors);
        fColorBuffer.position(0);

        ByteBuffer fbb = ByteBuffer.allocateDirect(fNormals.length*4);
        fbb.order(ByteOrder.nativeOrder());
        fNormalBuffer = fbb.asFloatBuffer();
        fNormalBuffer.put(fNormals);
        fNormalBuffer.position(0);

        ByteBuffer bbb = ByteBuffer.allocateDirect(bNormals.length*4);
        bbb.order(ByteOrder.nativeOrder());
        bNormalBuffer = bbb.asFloatBuffer();
        bNormalBuffer.put(bNormals);
        bNormalBuffer.position(0);

        fIndexBuffer = ByteBuffer.allocateDirect(fIndices.length);
        fIndexBuffer.put(fIndices);
        fIndexBuffer.position(0);

        bIndexBuffer = ByteBuffer.allocateDirect(bIndices.length);
        bIndexBuffer.put(bIndices);
        bIndexBuffer.position(0);
    }

    public void draw(GL10 gl)
    {
        gl.glVertexPointer(3, GL10.GL_FIXED, 0, fVertexBuffer);
        gl.glColorPointer(4, GL10.GL_FIXED, 0, fColorBuffer);
        //draw front of paper
        gl.glNormalPointer(GL10.GL_FLOAT, 0, fNormalBuffer);
        gl.glDrawElements(GL10.GL_TRIANGLES, 6, GL10.GL_UNSIGNED_BYTE, fIndexBuffer);
        //gl.glDrawElements(GL10.GL_LINE_LOOP, 6, GL10.GL_UNSIGNED_BYTE, fIndexBuffer);
        gl.glNormalPointer(GL10.GL_FLOAT, 0, bNormalBuffer);
        gl.glDrawElements(GL10.GL_TRIANGLES, 6, GL10.GL_UNSIGNED_BYTE, bIndexBuffer);
        //gl.glDrawElements(GL10.GL_LINE_LOOP, 6, GL10.GL_UNSIGNED_BYTE, bIndexBuffer);

    }

    public void resetColour()
    {

    }

    public void rotateAround(new Vector3(foldPoint1.x, foldPoint1.y, 0f), axis, foldAngle)
    {
        //TODO:IMPLEMENT THIS, IMPLEMENT IT REAL GOOD.
    }

}

