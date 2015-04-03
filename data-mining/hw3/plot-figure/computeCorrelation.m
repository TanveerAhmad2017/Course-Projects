
%preprocess data
%dataName = 'dataset1-KMean5';
%dataName = 'dataset1-EM';
%dataName = 'dataset1-DBS-0dot2';


%dataName = 'dataset2-KMean5';
%dataName = 'dataset2-EM';
%dataName = 'dataset2-DBS-0dot4';

data1 = load(strcat('..\processed-data\',dataName,'.txt'));
sortedData1 =sortrows(data1, 1:6);
clusters = sortedData1(:,6);
attributes = sortedData1(:,1:5);

instanceNum = size(clusters,1)

%compute incidence matrix
incidence = zeros(instanceNum,instanceNum);

for i=1:1:instanceNum
    for j = 1:1:instanceNum
        if(clusters(i,1) == clusters(j,1))
            incidence(i,j) = 1;
        end
    end
end

%compute proximity matrix
proximity = pdist2(attributes,attributes);
proximity = ones(instanceNum, instanceNum) - (proximity)/max(max(proximity));
%size(proximity)

C = corr2(incidence,proximity)

silh = silhouette(attributes, clusters);
mean(silh)