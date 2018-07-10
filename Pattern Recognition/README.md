
# Pattern recognition

## Semester project: Detection of blurred patches in images  


Project is created in Matlab R2018a.

*Idea:*
I aim to detect motion blur and out of focus blur on dataset contains 1000 images and also contains human labeled ground-truth images.
Source of dataset: http://www.cse.cuhk.edu.hk/leojia/projects/dblurdetect/dataset.html
I split this dataset on three parts: training, validation and testing dataset in 80:5:5.

My demo consist of 2 files m1.m and m2.m.
M1.m
I use superpixel function to split each image in ~100 superpixels
Extract multiple features from images.
	° histogram feateres (cca 8)
	° haralicks Texture features (cca 12)
Save this features in .mat format

M2.m
load data from .mat files
use PCA to reduce features
train classifier - non linear SVM and KNN(K-nearest neighbours) on training dataset 
adjust hyperparams of classifiers on validation set
Finnaly test correction of classification of classifiers. 
 
**Screens:**

![Scene](https://github.com/sarvasrobert/Scholar/blob/master/Graphics%20in%20GPU/GPU_PROJECT.png?raw=true)


	
