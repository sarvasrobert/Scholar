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
GLuint color_tex; //textura

/*
modelview matica 4x4
*/
// set matrix with an array
Matrix4x4 model_view, projection, modelmatrix1;
//normal matrix 3x3
float * normal_matrix = new float[9];


struct ObjectBuffers
{
	GLuint gTrianglesIB;
	GLuint gPositionsVB;
	GLuint gNormalsVB;
	GLuint gTexCoordsVB;
	GLuint gVertexAO;
	unsigned int faceCnt;
};

ObjectBuffers ob[4];

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

		//delete[] indexArr;

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

			//delete[] vPosArr;
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

			//delete[] texCoordArr;
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
	
	//This is the location of the camera projection matrix uniform variable in our program
	int loc = glGetUniformLocation(p, "camera_projection_matrix");
	glUniformMatrix4fv(loc, 1, GL_FALSE, projection.matrix);
	
	//kedze nepouzivame neuniformne skalovanie
	//skopirujeme cast modelovacej matice do normalovej
	for(int i=0; i<3; i++)
		for(int j=0; j<3; j++)
			normal_matrix[j + 3*i] = model_view.matrix[j + 4*i];

	/*
	posleme modelview a normalovu maticu shader programu p2
	*/
	//This is the location of the camera view matrix uniform variable in our program
	loc = glGetUniformLocation(p, "camera_model_view_matrix");
	glUniformMatrix4fv(loc, 1, GL_FALSE,  model_view.matrix);

	//This is the location of the normal matrix uniform variable in our program
	loc = glGetUniformLocation(p, "normal_matrix");
	glUniformMatrix3fv(loc, 1, GL_FALSE,  normal_matrix);

	// tu nastavujeme modelmatrix1 pre shader
	loc = glGetUniformLocation(p, "model_matrix1");
	glUniformMatrix3fv(loc, 1, GL_FALSE, modelmatrix1.matrix);

	for (int i = 0; i < 3; i++) {
		glBindVertexArray(ob[i].gVertexAO);
		// draw triangles from the currently bound VAO with current in-use shader
		glDrawElements(GL_TRIANGLES, ob[i].faceCnt * 3, GL_UNSIGNED_INT, NULL);
	}
   glutSwapBuffers();
}

void reshape(int w,int h)
{ 
   /* transformation of x and y from normalized device coordinates to window coordinates */
   glViewport (0, 0, (GLsizei) w, (GLsizei) h); 
   projection.LoadIdentity();


   //////////// nacitanie modelovej matice //////////
   modelmatrix1.LoadIdentity();
   modelmatrix1.Translate(1, 1, 1);

   //////////////////////////////////////////////////
   projection.setPerspective(/* field of view in degree */ 40.0, 
				  /* aspect ratio */(GLfloat) w/(GLfloat) h, 
				  /* Z near */0.1, 
				  /* Z far */10000.0); 
   //resetuj modelview maticu
	model_view.LoadIdentity();
	model_view.Translate(0.0, 0.0, -300.0);
	model_view.Rotate(30.0, 0.0, 1.0, 0.0);
}

int main(int argc, char **argv) {

	// init GLUT and create Window
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
	glutInitWindowPosition(100,100);
	glutInitWindowSize(800,600);
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
load texture
*/
	glEnable(GL_TEXTURE_2D);
	// Actual RGB data
	unsigned char * data = loadBMP("uvmapa.bmp");
	// Create one OpenGL texture
	//TO DO

	// Create one OpenGL texture
	glGenTextures(1, &color_tex);//vytvorime jednu texturu
							   // "Bind" the newly created texture : all future texture functions will modify this texture
	glBindTexture(GL_TEXTURE_2D, color_tex);
	// Give the image to OpenGL
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, NULL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 256, 256, 0, GL_BGR, GL_UNSIGNED_BYTE, NULL);


	loadScene(ob[0], "fishy.dae");				/////////  -1.0
	loadScene(ob[1], "tuna2.dae");			/////////  -1.0
	loadScene(ob[2], "31.dae");					////////// -300.0




	glUseProgram(p);

	//This is the location of the L_pos uniform variable in our program
	int loc = glGetUniformLocation(p, "L_pos");
	glUniform3f(loc, 10.0, 0.0, 1.0);

	loc = glGetUniformLocation(p, "tex");
	glUniform1i(loc, 0);

	// register callbacks
	
	glutDisplayFunc(renderScene);
	glutReshapeFunc(reshape);
	glutIdleFunc(renderScene);
	
	glEnable(GL_CULL_FACE);
    glCullFace(GL_BACK);
    glFrontFace(GL_CCW);
	


	glEnable(GL_DEPTH_TEST);

	// enter GLUT event processing cycle
	glutMainLoop();
	
	return 1;
}