/**
 * Created by james.smith on 2013-11-04.
 */
package ca.smiths.games.paperplanes;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;
import android.opengl.GLSurfaceView;

/**
 * Render a pair of tumbling cubes.
 */

class PaperRenderer implements GLSurfaceView.Renderer {
    public PaperRenderer(boolean useTranslucentBackground) {
        mTranslucentBackground = useTranslucentBackground;
        mCube = new Paper();
    }

    public void onDrawFrame(GL10 gl) {
        /*
         * Usually, the first thing one might want to do is to clear
         * the screen. The most efficient way of doing this is to use
         * glClear().
         */

        gl.glClear(GL10.GL_COLOR_BUFFER_BIT | GL10.GL_DEPTH_BUFFER_BIT);

        /*
         * Now we're ready to draw some 3D objects
         */

        mAngle += 1.2f;
        //mCube.rotation[0] = mAngle;
        mCube.rotation[1] = mAngle;
        gl.glMatrixMode(GL10.GL_MODELVIEW);
        gl.glLoadIdentity();
        gl.glTranslatef(mCube.position[0], mCube.position[1], mCube.position[2]);
        gl.glRotatef(mCube.rotation[0], 1, 0, 0);
        gl.glRotatef(mCube.rotation[1], 0, 1, 0);
        gl.glRotatef(mCube.rotation[2], 0, 0, 1);

        gl.glEnableClientState(GL10.GL_VERTEX_ARRAY);
        gl.glEnableClientState(GL10.GL_NORMAL_ARRAY);
        gl.glEnableClientState(GL10.GL_COLOR_ARRAY);

        mCube.draw(gl);

        gl.glDisableClientState(GL10.GL_VERTEX_ARRAY);
        gl.glDisableClientState(GL10.GL_NORMAL_ARRAY);
        gl.glDisableClientState(GL10.GL_COLOR_ARRAY);

    }

    public void onSurfaceChanged(GL10 gl, int width, int height) {
        gl.glViewport(0, 0, width, height);

         /*
          * Set our projection matrix. This doesn't have to be done
          * each time we draw, but usually a new projection needs to
          * be set when the viewport is resized.
          */

        float ratio = (float) width / height;
        gl.glMatrixMode(GL10.GL_PROJECTION);
        gl.glLoadIdentity();
        gl.glFrustumf(-ratio, ratio, -1, 1, 1, 10);
    }

    public void onSurfaceCreated(GL10 gl, EGLConfig config) {
        /*
         * By default, OpenGL enables features that improve quality
         * but reduce performance. One might want to tweak that
         * especially on software renderer.
         */
        gl.glDisable(GL10.GL_DITHER);

        /*
         * Some one-time OpenGL initialization can be made here
         * probably based on features of this particular context
         */
        gl.glHint(GL10.GL_PERSPECTIVE_CORRECTION_HINT,
                GL10.GL_FASTEST);

        if (mTranslucentBackground) {
            gl.glClearColor(0,0,0,0);
        } else {
            gl.glClearColor(0.07f, 0.639f, 0.7f, 1f);
        }
        gl.glMatrixMode(GL10.GL_MODELVIEW);
        gl.glEnable(GL10.GL_CULL_FACE);
        gl.glShadeModel(GL10.GL_FLAT);
        //lighting
        // Define the ambient component of the first light
        gl.glEnable(GL10.GL_LIGHTING);
        gl.glEnable(GL10.GL_LIGHT0);

        gl.glLoadIdentity();
    }
    private boolean mTranslucentBackground;
    private Paper mCube;
    private float mAngle;
}
