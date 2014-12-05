function plotFFBF()
    %Build Figure
    
    metrixString = {'SchedulerType'   'ScheduledProfit'  'UsedGreenEnergy'  'UsedBrownEnergyAmount'  'UsedBrownEnergyCost'   'ScheduledJobs.Count' 'ScheduledWorkloadUtilization'}
    metrixType = 2;
    metrix = cellstr(metrixString)
    
    figure1 = figure;
    set(figure1,'units','normalized','outerposition',[0 0 1 1]);

    axes1 = axes('Parent',figure1);
    box(axes1,'on');
    hold(axes1,'all');
    set(axes1,'FontSize',30,'FontWeight','bold');


    %set x,y axis
    x=[1:15]
    fffilename = strcat('data/firstfit_', metrix{metrixType} ,'.txt')
    ff = load(fffilename)
    bffilename = strcat('data/bestfit_', metrix{metrixType} ,'.txt')
    bf = load(bffilename)
%     ffError = 

    %plot and set font, line type
    p = plot(x(1,:),ff(1,:));
    set(p, 'Color', 'g', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', '<', 'MarkerSize', 10);

      p = plot(x(1,:),bf(1,:));
    set(p, 'Color', 'b', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', '*', 'MarkerSize', 10);



    %set x range and y range
    upper = max(max(ff (1,:)), max(bf(1,:)))
%     if 
%            upper = 2000
%     end
    axis([1 15 0 upper])  

    %set x tick and y tick
    %set(axes1,'YTick',[0.2,0.4,0.6,0.8,1.0],'YTickLabel',{'20%','40%','60%','80%','100%'},'XGrid','on','YGrid','on');
%     set(axes1,'XTick',[200,400,600,800,1000],'XTickLabel',{200,400,600,800,1000},'XGrid','on','YGrid','on');

    %set grid on 
    set(axes1,'XGrid','on','YGrid','on');

    %set legend
    legend(axes1,'show','Location','NorthWest','FontSize',10,'FontWeight','bold');
    legend('FirstFit','BestFit');

    %set x, y Label
    set(get(axes1,'XLabel'),'String','Arrival rate','FontSize',30,'FontWeight','bold');
    set(get(axes1,'YLabel'),'String', metrix{metrixType} ,'FontSize',30,'FontWeight','bold');

    %set title
    %title('10000 Attackers, 1000 Proxies');
end
 