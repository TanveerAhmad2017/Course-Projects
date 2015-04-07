%compute correlation and sihouette coefficient
%approaches

function computeCorrelation(dataName)

data1 = load(strcat('..\processed-data\',dataName,'.txt'));
sortedData1 =sortrows(data1, 7);
clusters = sortedData1(:,7);
attributes = sortedData1(:,2:6);

instanceNum = size(clusters,1);

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

disp(strcat('------------', dataName, '------------------'));

correlation = corr2(incidence,proximity)


silh = silhouette(attributes, clusters);
silhouetteDist = mean(silh)

end