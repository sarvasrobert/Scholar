
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%% dirs for training data
%imdir1 = 'C:\Users\Robert\Desktop\RO\testdata\INPUTS\1.treningData\';
%imdir2 = 'C:\Users\Robert\Desktop\RO\testdata\gt\1.treningData\';


%imdir1 = 'C:\Users\Robert\Desktop\RO\BlurDatasetImage\';
%imdir2 = 'C:\Users\Robert\Desktop\RO\BlurDatasetGT\gt\';


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%% dirs for validation data
%imdir1 = 'C:\Users\Robert\Desktop\RO\testdata\INPUTS\2.validationData\';
%imdir2 = 'C:\Users\Robert\Desktop\RO\testdata\gt\2.validationData\';

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%% dirs for test data
imdir1 = 'C:\Users\Robert\Desktop\RO\testdata\INPUTS\3.testData\';
imdir2 = 'C:\Users\Robert\Desktop\RO\testdata\gt\3.testData\';

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%% setting up mat files 
%%%% save to mat files
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
RFIN= zeros(1);
indR = 1;
MsizeFIN = [];
MMFIN=zeros(1,24);
%MHOG = [];
indM=1;
    
for c=1:53
    fprintf('compute %i.jpg ...\n',c);
    imfile1 = sprintf('%i.jpg',c);
    imfile2 = sprintf('%i.png',c); %%% FOR VALIDATION AND TEST DATA 
    %imfile2 = sprintf('%i.png',c+1); %%% FOR TRAINING DATA
    
    I = imread([imdir1, imfile1]);
    B = rgb2gray(I);
    %I2 = im2double(B);
    T = imread([imdir2, imfile2]);
    T2 = imbinarize(T);
    E = T2(:);
    

    
    %figure;
    %imshowpair(Gx, Gy, 'montage');
    %title('Directional Gradients: x-direction, Gx (left), y-direction, Gy (right), using Prewitt method');
    
    
    [L,N] = superpixels(I,100, 'Compactness',5);

    
%     figure;
%     BW = boundarymask(L);
%     imshow(imoverlay(I,BW,'cyan'),'InitialMagnification',67);
%     figure;
%     imshow(imoverlay(T,BW,'cyan'),'InitialMagnification',67);
    
    
    idx = label2idx(L);
    Stats = zeros(6,N);
    ft = zeros(14,N);
    
    M = zeros(c,N,24); 
    MsizeFIN(c) = N;
    
%     labs = vl_slic(I, 10,10);
%     contourImg = draw_contours(labs, I);
%     imshow(contourImg);
    
    
    for labelVal = 1:N
        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        %%%%%%%%  filling R matrix - 
        %%%%%%%%final correct values of superpixel by groundtruth
        actualpx = idx{labelVal};
        mean1 = mean(E(actualpx));
        if mean1 > 0.5
            RFIN(indR) = 1;
        else 
            RFIN(indR) = 0;
        end
        indR=indR+1;

        
        u = regionprops(L, 'PixelList');        
        u1 = u(labelVal).PixelList;
        %features = [];
%         for i=1:size(u1,1)
%             y = u1(i,1);
%             x = u1(i,2);
%             p(x,y) = I(x,y);
%         end 
        %features = extractHOGFeatures(p);
        U = B(u1);
        d = 11;

        [pdf1, pdf2, pdf3, pdf4] = GLDM(U, d);   %%% texture feature extraction GLDM
     
        %figure;imshow(U);title('Input Mammogram Image');

        %figure;
        %subplot(221);plot(pdf1);title('PDF Form 1');
        %subplot(222);plot(pdf2);title('PDF Form 2');
        %subplot(223);plot(pdf3);title('PDF Form 3');
        %subplot(224);plot(pdf4);title('PDF Form 4');
        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        %%%%%%%%code for extracting features from superpixel%%%%%%%%
        

        glcm = graycomatrix(U, 'offset', [0 1], 'Symmetric', true);
        xFeatures = 1:14;
        ftt = haralickTextureFeatures(glcm, 1:14);
        ft(:,labelVal) = ftt( xFeatures );
        Q = B(actualpx);
        Stats(:,labelVal) = chip_histogram_features( Q,'NumLevels',9,'G',[] );
        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        
        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        %%%%%%%%filling up matrix of superpixels M %%%%%%%%%%%%%%%%%
        
        for j=1:size(Stats,1)
            %M(c,labelVal,j) = Stats(j,labelVal);
            MMFIN(indM,j)=Stats(j,labelVal);
        end
        for j=size(Stats,1):size(ft,1)+size(Stats,1)-1
            %M(c,labelVal,j) = ft(j-size(Stats,1)+1,labelVal);
            MMFIN(indM,j)=ft(j-size(Stats,1)+1,labelVal);
        end
        %M(c,labelVal,20) = ft(14,labelVal);
        MMFIN(indM,j+1)=ft(14,labelVal);
%         inv = features(:)';
%         for a=1:size(features,2)
%             MHOG(indM,a) = features(1,a);
%         end
        
            %M(c,labelVal,j) = Stats(j,labelVal);
        MMFIN(indM,21)=pdf1(1,1);
        MMFIN(indM,22)=pdf2(1,1);
        MMFIN(indM,23)=pdf3(1,1);
        MMFIN(indM,24)=pdf4(1,1);
        
        indM = indM +1;
    end
end

save('FIN.mat', 'MMFIN' );
save('FINgt.mat', 'RFIN');
save('FINind.mat', 'MsizeFIN');
%save('FINHOG.mat', MHOG);


