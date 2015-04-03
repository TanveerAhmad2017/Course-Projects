%plot similarity matrix for all clustering approach

dataNames ={'dataset1-KMean5';  'dataset1-EM';  'dataset1-DBS-0dot2' ; 'dataset2-KMean5';  'dataset2-EM'; 'dataset2-DBS-0dot4'};

for i = 1:1:size(dataNames,1)
    plotSimilarity(char(dataNames(i,:)))
end

