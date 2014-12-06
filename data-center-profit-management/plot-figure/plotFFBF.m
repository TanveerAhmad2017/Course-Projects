function plotFFBF(metrixType, arrivalrate, setting)
    %Build Figure
    
    metrixString = {'SchedulerType'   'ScheduledProfit'  'UsedGreenEnergy'  'UsedBrownEnergyAmount'  'UsedBrownEnergyCost'   'ScheduledJobs.Count' 'ScheduledWorkloadUtilization' 'ArrivedWorkloadUtilization', 'AvgUnitBrownCost'}
    yAxisName = {'SchedulerType'   'Scheduled Profit'  'Used GreenEnergy'  'Used BrownEnergy Amount'  'Used BrownEnergy Cost'   'Scheduled Job Num' 'Scheduled WorkloadUtilization' 'ArrivedWorkloadUtilization', 'AvgUnitBrownCost'}
    

    metrix = cellstr(metrixString)
    
    figure1 = figure;
    set(figure1,'units','normalized','outerposition',[0 0 1 1], 'visible','off');

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
    set(p, 'Color', 'b', 'LineWidth', 3, 'linestyle','--');
    set(p, 'Marker', '<', 'MarkerSize', 10);
 

    if   metrixType ~= 8
        p = plot(x(1,:),bf(1,:));
        set(p, 'Color', 'g', 'LineWidth', 3, 'linestyle','--');
        set(p, 'Marker', '*', 'MarkerSize', 10);
        
    end
    
    errorbar(x,ff(1,:),ff(2,:),'Color', 'b','LineWidth', 2 )

    if   metrixType ~= 8
    errorbar(x,bf(1,:),bf(2,:),'Color', 'g','LineWidth', 2)
    end

    %set x range and y range
    upper = max(max(ff (1,:)), max(bf(1,:)))
%     if 
%            upper = 2000
%     end
    switch metrixType
    case 7 
        upper = 1;
    case 8
        upper = ceil( upper/1);
    case 9
        upper = ceil( upper/1);
        otherwise
        upper = ceil(upper/100)*100;        
    end
            
    axis([1 15.1 0 upper])  
    

    %set x tick and y tick
    %set(axes1,'YTick',[0.2,0.4,0.6,0.8,1.0],'YTickLabel',{'20%','40%','60%','80%','100%'},'XGrid','on','YGrid','on');
   set(axes1,'XTick',[3,6,9,12,15],'XTickLabel',[0.3,0.6,0.9,1.2,1.5].*arrivalrate,'XGrid','on','YGrid','on');

    %set grid on 
    set(axes1,'XGrid','on','YGrid','on');

    %set legend
   
   if metrixType == 8 
        legend(axes1,'show','Location','SouthEast','FontSize',10,'FontWeight','bold');
        leg=    legend('Workload');
        set(leg,'Location','SouthEast');
   else 
        legend(axes1,'show','Location','SouthEast','FontSize',10,'FontWeight','bold');
        leg=    legend('FirstFit','BestFit');
        set(leg,'Location','SouthEast');
   end

    %set x, y Label
    set(get(axes1,'XLabel'),'String','Arrival rate','FontSize',30,'FontWeight','bold');
    set(get(axes1,'YLabel'),'String', metrix{metrixType} ,'FontSize',30,'FontWeight','bold');

    %set title
    %title('10000 Attackers, 1000 Proxies');
    set(gcf, 'PaperPosition', [0 0 13 7]); %Position plot at left hand corner with width 5 and height 5.
    set(gcf, 'PaperSize', [13 7]); %Set the paper to have width 5 and height 5.
    %saveas(gcf, 'SolarTrace_High', 'pdf') %Save figure
    filename = strcat('.\figures\FFBF_', metrixString{metrixType},setting)
    saveas(gcf, filename, 'pdf') %Save figure  
    saveas(gca, strcat(filename, '.eps'),'psc2') %Save figure 
    
     filename = strcat('C:\Users\hwang14\Dropbox\Meerkats\greenSlot\greenSlot-CS773\Figures\FFBF_', metrixString{metrixType},setting)
    saveas(gcf, filename, 'pdf') %Save figure  
    saveas(gca, strcat(filename, '.eps'),'psc2') %Save figure 
    
end
 