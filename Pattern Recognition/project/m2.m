

%% loading motions training data
MM = [];
R = [];
Msize = [];
load('FOCUS.mat');
load('FOCUSgt.mat');
%% loading focus training data 
MMALL = [];
RALL = [];
MsizeALL = [];
load('MOTIONS.mat');
load('MOTIONSgt.mat');
%% loading validation data
RVAL =[];
MMVAL =[];
MsizeVAL = [];
load('VAL.mat');
load('VALgt.mat');
%% loading test data 
RFIN = [];
MMFIN = [];
MsizeFIN = [];
load('FIN.mat');
load('FINgt.mat');
%%

F =  [R RALL];
Mt = [MM; MMALL];


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%% PCA %%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

[COEFF,SCORE,latent] = pca(MM);
%figure, plot(SCORE(:,1), SCORE(:,2),'r.'); 

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%% SVM %%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% figure;
% plot(SCORE(:,1),SCORE(:,2),'r.','MarkerSize',15)
% hold on
% plot(SCORE(:,1),SCORE(:,2),'b.','MarkerSize',15)
% ezpolar(@(x)1);ezpolar(@(x)2);
% axis equal
% hold off

data3 = SCORE;
theclass = R';
data4 = data3;
data2 = data3;
%Fit nonlinear SVM 
c0 = fitcsvm(data3,theclass,'KernelFunction','rbf','Standardize',true,'ClassNames',[1,0]);
% Predict scores over the grid

%[x1Grid,x2Grid] =  meshgrid(min(data3(:,1)):d:max(data3(:,1)),...
%                          min(data3(:,2)):d:max(data3(:,2)));
%xGrid = [x1Grid(:),x2Grid(:)];

[label,scores] = predict(c0,data3);
C = label==theclass;
result = mean(C);
fprintf('svm train correctly at %f%%\n', result*100);

% Plot the data and the decision boundary
% figure;
% gscatter(data3(:,1),data3(:,2),theclass,'rb','.');
% hold on
% ezpolar(@(x)1);
% plot(data3(c0.IsSupportVector,1),data3(c0.IsSupportVector,2),'ko');
% axis equal
% hold off

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

k=5;
Mdl = fitcknn(data4,theclass,'NumNeighbors',k,'Standardize',1);

[label1,scores1] = predict(Mdl,data4);   
C = label1 == theclass;
res = mean(C);
fprintf('KNN train correctly at %f%%\n', res*100);

% figure; hold on;
% for i=1:size(C,1)
%     if C(i) == 0
%         plot(data3(i,1), data3(i,2), 'b.');
%     else
%         plot(data3(i,1), data3(i,2), 'g+');
%     end
% end
% hold off;

Linear = fitclinear(data2,theclass);
[label3,scores3] = predict(Linear,data2);   
C = label3 == theclass;
res = mean(C);
fprintf('Linear Classifier train correctly at %f%%\n', res*100);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%% end of training part 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%% testing on validation data %%%%%%%%%%%%



[COEFF,SCORE,latent] = pca(MMVAL);
data5 = SCORE;
theclass1 = RVAL';

[label,scores] = predict(c0,data5);
C = label==theclass1;
result = mean(C);
fprintf('svm train correctly at %f%%\n', result*100);

[label1,scores1] = predict(Mdl,data5);   
C = label1 == theclass1;
res = mean(C);
fprintf('KNN train correctly at %f%%\n', res*100);


[label3,scores3] = predict(Linear,data5);   
C = label3 == theclass1;
res = mean(C);
fprintf('Linear Classifier train correctly at %f%%\n', res*100);




[COEFF,SCORE,latent] = pca(MMFIN);
data6 = SCORE;
theclass4 = RFIN';

[label,scores] = predict(c0,data6);
C = label==theclass4;
result = mean(C);
fprintf('svm train correctly at %f%%\n', result*100);

[label5,scores5] = predict(Mdl,data6);   
C = label5 == theclass4;
res = mean(C);
fprintf('KNN train correctly at %f%%\n', res*100);


[label6,scores6] = predict(Linear,data6);   
C = label6 == theclass4;
res = mean(C);
fprintf('Linear Classifier train correctly at %f%%\n', res*100);

% P =0; N=0;
% TP=0;TN=0;FP=0;FN=0;
% for i=1:size(theclass1,1)
%     if theclass1(i,1) == 1
%         if label3(i,1) ==1
%             TP = TP +1;
%         else
%             TN = TN + 1;
%         end
%         P=P +1;
%     else
%         if label3(i,1) ==0
%             FP = FP +1;
%         else
%             FN = FN + 1;
%         end
%         N= N +1;
%     end
% end
% 
% TPR = TP / P;
% TNR = TN / N;
% the = [TPR TNR];
% ROCout = roc_curve(the);
[X,Y]= perfcurve(theclass4,scores6(:,1),1);
plot(X,Y);
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%% end of VALIDATION  part 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

