#include <iostream>
#include <stdlib.h>
#include "GL/glew.c"
#include "GL/glut.h"
#include "tools/glslang.cpp"
#include "tools/matrix4x4.cpp"
#include "tools/loadBMP.cpp"
#include "tools/OBJ_Loader.h"

#pragma comment(lib, "assimp.lib")

#include "Assimp/assimp.hpp"	
#include "Assimp/aiPostProcess.h"
#include "Assimp/aiScene.h"

GLuint p;  //shader program
GLuint color_tex, color_tex1, color_tex2, color_tex3, color_tex4, color_tex5; //textura

/*
modelview matica 4x4
*/
// set matrix with an array
Matrix4x4 model_view, projection, modelmatrix1, camera;
//normal matrix 3x3
float * normal_matrix = new float[9];
float uhol = 0;
bool uholp = true;
float dopredu = 0.0;
char key;
bool zapnihmlu = true;
bool linear = true;
float makrela = 0.0;
bool makrelasmer = true;
float posunZ = 0.0;
float posunY = 0.0;
float rotujX = 0.0;
float rotujY = 0.0;
int asciiValue = 0;
bool testA = false;
float uholkamery = 0;
bool rotujkameru = false;


struct ObjectBuffers
{
	GLuint gTrianglesIB;
	GLuint gPositionsVB;
	GLuint gNormalsVB;
	GLuint gTexCoordsVB;
	GLuint gVertexAO;
	unsigned int faceCnt;
};

ObjectBuffers ob[6];
float* vPosArr = 0;

void loadScene(ObjectBuffers &o, std::string name)
{
		std::string pFile = name; 
 
		// Create an instance of the Importer class
		Assimp::Importer importer;

		const aiScene* scene = importer.ReadFile( pFile, aiProcessPreset_TargetRealtime_Fast);

			// If the import failed, report it
		if(!scene)
		{
			printf("%s\n", importer.GetErrorString());
			return;
		}

		// generate VBOs
		aiMesh* tmpMesh = scene->mMeshes[0];
		
		// triangles array
		unsigned int* indexArr = (unsigned int *)malloc(sizeof(unsigned int) * tmpMesh->mNumFaces * 3);
		int index = 0;
		int vertCnt = 0;

		const struct aiFace* face;

		for (unsigned int t = 0; t < tmpMesh->mNumFaces; ++t)
		{
			face = &tmpMesh->mFaces[t];

			memcpy(&indexArr[index], face->mIndices, 3 * sizeof(unsigned int));
			index += 3;
		}
		o.faceCnt = tmpMesh->mNumFaces;

		//element array buffer for faces
		glGenBuffers(1, &o.gTrianglesIB);
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, o.gTrianglesIB);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * o.faceCnt* 3, indexArr, GL_STATIC_DRAW);

		delete[] indexArr;

		//array buffer for vertex positions
		if (tmpMesh->HasPositions())
		{
			// save position arrays for animation
			vPosArr = (float *)malloc(sizeof(float) * tmpMesh->mNumVertices * 3);
			index = 0;
			for (unsigned int t = 0; t < tmpMesh->mNumVertices; ++t)
			{
				memcpy(&vPosArr[index], &tmpMesh->mVertices[t], 3 * sizeof(float));
				index += 3;
			}
			vertCnt = tmpMesh->mNumVertices;

			glGenBuffers(1, &o.gPositionsVB);
			glBindBuffer(GL_ARRAY_BUFFER, o.gPositionsVB);
			glBufferData(GL_ARRAY_BUFFER, sizeof(float)*3*vertCnt, vPosArr, GL_STATIC_DRAW);

			delete[] vPosArr;
		}

		
		if (tmpMesh->HasNormals())
		{
			glGenBuffers(1, &o.gNormalsVB);
			glBindBuffer(GL_ARRAY_BUFFER, o.gNormalsVB);
			glBufferData(GL_ARRAY_BUFFER, sizeof(float)*3*vertCnt, tmpMesh->mNormals, GL_STATIC_DRAW);

		}

		float * texCoordArr = 0;
		float uv[2];
		if (tmpMesh->HasTextureCoords(0))
		{
			texCoordArr = (float *)malloc(sizeof(float) * vertCnt * 2);
			index = 0;
			for (unsigned int t = 0; t < vertCnt; ++t)
			{
				uv[0] = tmpMesh->mTextureCoords[0][t].x;
				uv[1] = tmpMesh->mTextureCoords[0][t].y;
				memcpy(&texCoordArr[index], &uv, 2 * sizeof(float));
				index += 2;
			}

			glGenBuffers(1, &o.gTexCoordsVB);
			glBindBuffer(GL_ARRAY_BUFFER, o.gTexCoordsVB);
			glBufferData(GL_ARRAY_BUFFER, sizeof(float)*2*vertCnt, texCoordArr, GL_STATIC_DRAW);

			delete[] texCoordArr;
		}

		importer.FreeScene();

		//create vertex array object from array buffers
		glGenVertexArrays (1, &o.gVertexAO);
		glBindVertexArray (o.gVertexAO); 
		//TO DO


		glGenVertexArrays(1, &o.gVertexAO);
		glBindVertexArray(o.gVertexAO);

		glBindBuffer(GL_ARRAY_BUFFER, o.gPositionsVB);
		glEnableVertexAttribArray(0);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, NULL);

		glBindBuffer(GL_ARRAY_BUFFER, o.gNormalsVB);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 0, NULL);

		glBindBuffer(GL_ARRAY_BUFFER, o.gTexCoordsVB);
		glEnableVertexAttribArray(2);
		glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 0, NULL);
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, o.gTrianglesIB);
	}



void renderScene(void) {

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glUseProgram(p);

	if (uholp == true)  {  uhol = (uhol + 0.1);  }
	else				{  uhol = (uhol - 0.1);  }

	if (uhol > 30.0)    {  uholp = false;         }
	if (uhol < -30.0)   {  uholp = true;          }
	

	for (int i = 0; i < 6; i++) {

		//This is the location of the camera projection matrix uniform variable in our program

		int loc = glGetUniformLocation(p, "camera_projection_matrix");
		glUniformMatrix4fv(loc, 1, GL_FALSE, projection.matrix);

		loc = glGetUniformLocation(p, "posunZ");
		glUniform1f(loc, posunZ);

		loc = glGetUniformLocation(p, "linear");
		glUniform1f(loc, linear);

		camera.LoadIdentity();
		//camera.Translate( 0.0,0.0, posunZ);
		//camera.Translate( 0.0,0.0, posunZ);
		camera.Rotate(rotujX, 1.0, 1.0, 0.0);
		camera.Rotate(rotujY, 1.0, 0.0, 0.0);

		for (int u = 0; u < 3; u++)
		{
			for (int j = 0; j < 3; j++)
			{
				normal_matrix[j + 3 * u] = camera.matrix[j + 4 * u];
			}
		}

		loc = glGetUniformLocation(p, "camera");
		glUniformMatrix4fv(loc, 1, GL_FALSE, camera.matrix);

		if (i == 0) {
			model_view.LoadIdentity();
			model_view.Scale(0.00001, 0.00001, 0.00001);
			model_view.Translate(-120.0, 0.0, -260.0);
			//model_view.Rotate(0.0, 0.0, 1.0, 0.0);
			if (uholp == true)  { model_view.Rotate(uhol, 0.0, 1.0, 0.0);   }
			else				{ model_view.Rotate(uhol, 0.0, 1.0, 0.0);   }
			glBindTexture(GL_TEXTURE_2D, color_tex);
		}
		else if (i == 1) {
			model_view.LoadIdentity();
			model_view.Scale(0.01, 0.01, 0.01);
			model_view.Translate(-100.0, 00.0, -200.0);
			model_view.Rotate(50.0, 0.0, 1.0, 0.0);
			if (uholp == true)  { model_view.Rotate(uhol, 0.0, 1.0, 0.0);   }
			else				{ model_view.Rotate(uhol, 0.0, 1.0, 0.0);   }
			glBindTexture(GL_TEXTURE_2D, color_tex);
		}
		else if (i == 2)
		{
			model_view.LoadIdentity();
			model_view.Scale(1.0, 1.0, 1.0);
			makrela = dopredu*0.01;
			if (makrelasmer == false) {
				if (makrela > 10.0) { makrelasmer = true; }
				dopredu -= 0.2;
				makrela = dopredu*0.01;
				model_view.Translate(-3.0 - makrela, 1.0, -6.0);
				model_view.Rotate(-50.0, 0.0, 1.0, 0.0);
			} else {
				if (makrela < -10.0) { makrelasmer = false; }
				dopredu += 0.2;
				makrela = dopredu*0.01;
				model_view.Translate(-3.0 - makrela, 1.0, -6.0);
				model_view.Rotate(50.0, 0.0, 1.0, 0.0);
			}

			if (uholp == true)   { model_view.Rotate(uhol, 0.0, 1.0, 0.0);  }
			else				 { model_view.Rotate(uhol, 0.0, 1.0, 0.0);  }
			glBindTexture(GL_TEXTURE_2D, color_tex1);
		}
		else if (i == 3)
		{	
			model_view.LoadIdentity();
			model_view.Scale(100.0, 100.0, 100.0);
			model_view.Translate(+5.0+(dopredu*0.001), 1.0, -5.0 + (dopredu*0.001));
			model_view.Rotate(30.0, 0.0, 1.0, 0.0);
			if (uholp == true)    { model_view.Rotate(uhol, 1.0, 0.0, 0.0);  }
			else				  { model_view.Rotate(uhol, 1.0, 0.0, 0.0);  }
			glBindTexture(GL_TEXTURE_2D, color_tex2);
		}

		else if (i == 4)
		{
			model_view.LoadIdentity();
			model_view.Scale(60.0, 40.0, 60.0);
			model_view.Translate(100.0, -130.0, 100.0-(dopredu*0.1));
			model_view.Rotate(-90.0, 1.0, 0.0, 1.0);
			model_view.Rotate(20.0, 0.0, 1.0, 0.0);
			glBindTexture(GL_TEXTURE_2D, color_tex3);
		}

		else if (i == 5)
		{
			model_view.LoadIdentity();
			model_view.Scale(100, 100, 100);
			model_view.Translate(-50, 10, -20);
			model_view.Rotate(90.0, 1.0, 0.0, 0.0);
			//model_view.Rotate(-90.0, 1.0, 0.0, 1.0);

			glBindTexture(GL_TEXTURE_2D, color_tex4);
			
		}
		//kedze nepouzivame neuniformne skalovanie
		//skopirujeme cast modelovacej matice do normalovej
		if (rotujkameru == true) {
			if (i == 2) {
				uholkamery += 0.01;
				model_view.Rotate(uholkamery, 0.0, 0.0, 1.0);	}
			else {
				uholkamery += 0.01;
				model_view.Rotate(uholkamery, 0.0, 1.0, 0.0);	}
		}

		for (int u = 0; u < 3; u++)
		{
			for (int j = 0; j < 3; j++)
			{
				normal_matrix[j + 3 * u] = model_view.matrix[j + 4 * u];
			}
		}
		//normal_matrix[j + 3 * i] = modelmatrix1.matrix[j + 4 * i];
		/*
		posleme modelview a normalovu maticu shader programu p2
		*/
		//This is the location of the camera view matrix uniform variable in our program
		loc = glGetUniformLocation(p, "camera_model_view_matrix");
		glUniformMatrix4fv(loc, 1, GL_FALSE, model_view.matrix);

		//This is the location of the normal matrix uniform variable in our program
		loc = glGetUniformLocation(p, "normal_matrix");
		glUniformMatrix3fv(loc, 1, GL_FALSE, normal_matrix);

		// tu nastavujeme modelmatrix1 pre shader
		loc = glGetUniformLocation(p, "model_matrix1");
		glUniformMatrix4fv(loc, 1, GL_FALSE, modelmatrix1.matrix);

		if (zapnihmlu == true) {
			loc = glGetUniformLocation(p, "hmla");
			glUniform1f(loc, 1.0);
		}
		else {
			loc = glGetUniformLocation(p, "hmla");
			glUniform1f(loc, 0.0);
		}


		glBindVertexArray(ob[i].gVertexAO);
		// draw triangles from the currently bound VAO with current in-use shader
		glDrawElements(GL_TRIANGLES, ob[i].faceCnt * 3, GL_UNSIGNED_INT, NULL);
		glBindVertexArray(0);
	}
   glutSwapBuffers();
}

void reshape(int w,int h)
{ 
   /* transformation of x and y from normalized device coordinates to window coordinates */
   glViewport (0, 0, (GLsizei) w, (GLsizei) h); 
   projection.LoadIdentity();
   //projection.Translate(0.0, 0.0, posunZ);
   //////////////////////////////////////////////////
   projection.setPerspective(/* field of view in degree */ 55.0, 
				  /* aspect ratio */(GLfloat) w/(GLfloat) h, 
				  /* Z near */0.001, 
				  /* Z far */100000.0); 

   //resetuj modelview maticu
   projection.Translate(0.0, 0.0, posunZ);
}

//ToDo definujte Zap./Vyp. efektu ambient occlusion po stalceni tlacidla a
void keyPressed(unsigned char key, int x, int y) {
	//int location = glGetUniformLocation(...);
	//if (key == 'a' && rotujkameru == false) {	// If they ‘a’ key was pressed
	// Perform action associated with the ‘a’ key
	//	rotujkameru = true;
	//}
	//else if (key == 'a' && rotujkameru == true) {
	//	rotujkameru = false;
	//}
	if (key == 'a' || key == 'A') { rotujX -= 1.0; }
	else if (key == 'd' || key == 'D') { rotujX += 1.0; }
	else if (key == 'w' || key == 'W') { rotujY -= 1.0; }
	else if (key == 'S' || key == 's') { rotujY += 1.0; }
	if (key == 'k')  { posunZ += 10.0; }
	if (key == 'l') { posunZ -= 10.0; }
	if (key == 'h' || key == 'H') { if (zapnihmlu == true) { zapnihmlu = false; } else { zapnihmlu = true; } }
	if (key == 'g' || key == 'G') { if (linear == true) { linear = false; } else { linear = true; } }
}

int main(int argc, char **argv) {

	// init GLUT and create Window
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
	glutInitWindowPosition(100,100);
	glutInitWindowSize(1200,800);
	glutCreateWindow("OpenGL");

	// init GLEW to support extensions
	glewInit();
	if (glewIsSupported("GL_VERSION_3_3"))
        printf("Ready for OpenGL 3.3\n");
    else {
        printf("OpenGL 3.3 not supported\n");
        exit(1);
    }

	p = createShaderProgram("./shaders/shader.vert","./shaders/shader.frag");

	/*
    load textures and load objects
    */
	glEnable(GL_TEXTURE_2D);
	///////////////////////////////////////////////////////

	// Actual RGB data
	unsigned char * data = loadBMP("shark_tex2.bmp");// shark_tex.bmp");
	// Create one OpenGL texture
	//TO DO

	// Create one OpenGL texture
	glGenTextures(1, &color_tex);//vytvorime jednu texturu
							   // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data);


	loadScene(ob[0], "2.dae");				/////////  -150.0
	glBindVertexArray(0);

	loadScene(ob[1], "earth.dae");
	glBindVertexArray(0);
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////

	unsigned char * data1 = loadBMP("tuna_tex.bmp");
	// Create one OpenGL texture
	//TO DO

	// Create one OpenGL texture
	glGenTextures(1, &color_tex1);//vytvorime jednu texturu
								 // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex1);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data1);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data1);

	loadScene(ob[2], "tuna2.dae");			/////////  -1.0
	glBindVertexArray(0);
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////

	unsigned char * data2 = loadBMP("dolphin.bmp");
	// Create one OpenGL texture
	//TO DO

	// Create one OpenGL texture
	glGenTextures(1, &color_tex2);//vytvorime jednu texturu
								 // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex2);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data2);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data2);

	loadScene(ob[3], "DOLPHIN.dae");				////////// -4000.0
	glBindVertexArray(0);

	unsigned char * data3 = loadBMP("sand1.bmp");// shark_tex.bmp");
	// Create one OpenGL texture
	glGenTextures(1, &color_tex3);//vytvorime jednu texturu
								  // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex3);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data3);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data3);

	loadScene(ob[4], "3d-model.dae");
	//loadScene(ob[4], "earth.dae");
	glBindVertexArray(0);
	///////////////////////////////////////////////////////////////////////////////////////////////////

	unsigned char * data4 = loadBMP("1.bmp");// shark_tex.bmp");
												 // Create one OpenGL texture
	glGenTextures(1, &color_tex4);//vytvorime jednu texturu
								  // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex4);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data4);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data4);


	unsigned char * data5 = loadBMP("2.bmp");// shark_tex.bmp");
											 // Create one OpenGL texture
	glGenTextures(1, &color_tex5);//vytvorime jednu texturu
								  // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex5);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data5);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, data5);

	loadScene(ob[5], "submarine.dae");
	glBindVertexArray(0);
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////
	///////////////////////////////////////////////////////

	glUseProgram(p);

	//This is the location of the L_pos uniform variable in our program
	int loc = glGetUniformLocation(p, "L_pos");
	glUniform3f(loc, 10.0, 50.0, 1.0);


	loc = glGetUniformLocation(p, "L1_pos");
	glUniform3f(loc, 20.0, 20.0, 1.0);

	loc = glGetUniformLocation(p, "L2_pos");
	glUniform3f(loc, -10.0, 30.0, 1.0);

	loc = glGetUniformLocation(p, "tex");
	glUniform1i(loc, 0);

	// register callbacks
	
	glutDisplayFunc(renderScene);
	glutKeyboardFunc(keyPressed);
	glutReshapeFunc(reshape);
	glutIdleFunc(renderScene);



	glFogi(GL_FOG_COORD_SRC, GL_FRAGMENT_DEPTH);
	glEnable(GL_CULL_FACE);
    glCullFace(GL_BACK);
    glFrontFace(GL_CCW);
	

	glClearColor(0.0, 0.0, 0.55, 1);// blue color of background
	glClearDepth(1);
	glEnable(GL_DEPTH_TEST);
	//glDepthFunc(GL_ALWAYS);

	// enter GLUT event processing cycle
	glutMainLoop();
	
	return 1;
}