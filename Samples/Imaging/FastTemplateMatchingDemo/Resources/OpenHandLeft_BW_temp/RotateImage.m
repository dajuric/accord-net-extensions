function RotateImage(imgFileName)

	coreName = imgFileName(1:length(imgFileName)-3-1);
	img = imread(imgFileName);
	
    rotations = -90:5:90;
    background = 0;
    
    idx=1;
    for r = rotations
        imR = imrotate(img, r, 'bilinear');
        
        maskR = ~imrotate(true(size(img)),r, 'bilinear');
        imR(maskR & ~imclearborder(maskR)) = background;
        
		imwrite(imR, strcat(coreName, '_Rotated_', num2str(idx), '.bmp'));
        idx = idx + 1;
    end
end