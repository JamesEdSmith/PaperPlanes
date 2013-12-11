/**
 * Created by james.smith on 2013-11-04.
 */
package ca.smiths.games.paperplanes;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;
import android.opengl.GLSurfaceView;

import java.lang.Math;
import java.nio.FloatBuffer;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.math.Vector3;

/**
 * Render a pair of tumbling cubes.
 */

class PaperRenderer implements GLSurfaceView.Renderer {

    private Paper movingFold;
    private float foldAngle;
    private float foldTracker;
    private boolean mTranslucentBackground;
    private Paper mCube;
    private float mAngle;
    private Vector2 foldPoint1;
    private Vector2 foldPoint2;

    public PaperRenderer(boolean useTranslucentBackground) {
        mTranslucentBackground = useTranslucentBackground;
        mCube = new Paper();
    }

    public void onDrawFrame(GL10 gl) {
        update();

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

    void update()
    {
        if(movingFold)
            moving();
        else
            folding();
    }

    void moving()
    {
        Vector3 axis = new Vector3(foldPoint1.x - foldPoint2.x, foldPoint1.y - foldPoint2.y, 0f);
        float dx = Math.pow(axis.x, 2);
        float dy = Math.pow(axis.y, 2);
        float dz = Math.pow (axis.z, 2);
        float magnitude = (float)Math.pow((dx + dy + dz), 0.5);

        axis.scl(magnitude);
        //TODO: ADD THIS BACK IN
        //foldAngle = Input.GetAxis("MouseX") * 4f;

        if(Math.abs(foldAngle + foldTracker) <= 180)
        {
            foldTracker += foldAngle;
        }
        else
        {
            if(foldAngle + foldTracker >= 180f)
            {
                foldAngle = 180f - foldTracker;
                foldTracker = 180f;
            }
            else
            {
                foldAngle = -180f - foldTracker;
                foldTracker = -180f;
            }

        }

        movingFold.rotateAround(new Vector3(foldPoint1.x, foldPoint1.y, 0f), axis, foldAngle);
        //foreach(Transform child in movingFold.transform)
        //child.RotateAround(new Vector3(foldPoint1.x, foldPoint1.y, 0f), axis, foldAngle);

        /*if(Input.GetButton("Fire1"))
        {
            movingFold = null;
            moveClick = true;
            foldTracker = 0f;
        }*/
    }

    void folding()
    {
        if(!moveClick)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //new Vector3(Input.GetAxis("MouseX"), Input.GetAxis("MouseY"), 0)
            if ( Physics.Raycast(ray, out hit, 99999))
            {
                if(lastHit != hit.collider.gameObject && lastHit)
                {
                    lastHit.renderer.material.color = Color.white;
                }

                if ( hit.collider.gameObject.renderer )
                {

                    hit.collider.gameObject.renderer.material.color = new Color(0.19f, 0.55f, 0.91f, 1.0f);
                    hitLastUpdate = true;

                }

                if(Input.GetButton("Fire1"))
                {
                    if(!prevClick)
                    {
                        //foldPoint1 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        foldPoint1 = new Vector2(hit.point.x, hit.point.y);
                    }
                    prevClick = true;
                    hit.collider.gameObject.renderer.material.color = new Color(1.0f, 0.55f, 0.91f, 1.0f);
                }
                else
                {
                    if (prevClick)
                    {
                        //foldPoint2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        foldPoint2 = new Vector2(hit.point.x, hit.point.y);
                        foldLine((MeshCollider)hit.collider);
                        hit.collider.gameObject.renderer.material.color = new Color(0.19f, 0.55f, 0.91f, 1.0f);
                    }
                    prevClick = false;
                }

                lastHit = hit.collider.gameObject;
            }
            else if (hitLastUpdate && lastHit != null)
            {
                lastHit.renderer.material.color = Color.white;
                hitLastUpdate = false;
            }
            // paper turning
            bool moved = false;

            if(refacedLastUpdate)
            {
                if(Input.GetAxis("Horizontal") == 0)
                    refacedLastUpdate = false;
            }
            else
            {
                if(Input.GetAxis("Horizontal") > 0)
                {
                    polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.white;
                    camFoldIndex++;
                    if(camFoldIndex >= polygonTester.foldSections.Count)
                    {
                        camFoldIndex = 0;
                    }
                    polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.green;
                    moved = true;
                }
                else if (Input.GetAxis("Horizontal") < 0)
                {
                    polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.white;
                    camFoldIndex--;
                    if(camFoldIndex < 0 )
                    {
                        camFoldIndex = polygonTester.foldSections.Count -1;
                    }
                    polygonTester.foldSections[camFoldIndex].renderer.material.color = Color.green;
                    moved = true;
                }

                if (moved)
                {
                    Vector3 shift = Vector3.zero - polygonTester.foldSections[camFoldIndex].transform.position;

                    str2 = "shift " + shift.x + " " + shift.y + " " + shift.z;

                    foreach(GameObject go in polygonTester.foldSections)
                    {
                        go.transform.position += shift;
                    }

                    refacedLastUpdate = true;
                }
            }
        }
        else
        {
            if(!Input.GetButton("Fire1"))
            {
                moveClick = false;
            }
        }
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
        gl.glLineWidth(2f);

        //lighting
        // Define the ambient component of the first light
        gl.glEnable(GL10.GL_LIGHTING);
        gl.glEnable(GL10.GL_LIGHT0);
        gl.glEnable(GL10.GL_LIGHT1);
        gl.glEnable(GL10.GL_LIGHT2);

        float lightColor[] = {0.07f, 0.639f, 0.7f, 1f};
        FloatBuffer light0Ambient = FloatBuffer.wrap(lightColor);
        float light1PositionVector[] = {1f, 0f, 0f, 0f};
        FloatBuffer light1Position = FloatBuffer.wrap(light1PositionVector);
        float light2PositionVector[] = {-1f, 0f, 0f, 0f};
        FloatBuffer light2Position = FloatBuffer.wrap(light2PositionVector);

        gl.glLightfv(GL10.GL_LIGHT1, GL10.GL_AMBIENT, light0Ambient);
        gl.glLightfv(GL10.GL_LIGHT1, GL10.GL_POSITION, light1Position);
        gl.glLightfv(GL10.GL_LIGHT2, GL10.GL_AMBIENT, light0Ambient);
        gl.glLightfv(GL10.GL_LIGHT2, GL10.GL_POSITION, light2Position);
    }
}
