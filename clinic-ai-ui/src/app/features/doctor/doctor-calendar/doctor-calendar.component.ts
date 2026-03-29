import { Component, OnInit } from '@angular/core';
import { CalendarOptions } from '@fullcalendar/core';
import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { AppointmentService } from '../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { AppointmentModel } from '../../../shared/appointment-model/appointment-model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-doctor-calendar',
  standalone: true,
  imports: [FullCalendarModule, CommonModule, AppointmentModel, FormsModule],
  templateUrl: './doctor-calendar.component.html',
  styleUrl: './doctor-calendar.component.css',
})
export class DoctorCalendarComponent implements OnInit {

  viewMode: 'calendar' | 'list' = 'calendar';

  appointments: any[] = [];
  selectedEvent: any = null;

  selectedDate: string | null = null;
  filteredAppointments: any[] = [];

  statusFilter: 'All' | 'Scheduled' | 'Cancelled' = 'All';
  searchText: string = '';

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'timeGridWeek',

    slotMinTime: '08:00:00',
    slotMaxTime: '20:00:00',
    slotDuration: '00:20:00',
    allDaySlot: false,

    nowIndicator: true,
    height: 'auto',
    scrollTime: '09:00:00',

    editable: true,

    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'timeGridWeek,dayGridMonth'
    },

    eventClick: (info) => {
      this.selectedEvent = info.event;
    },

    eventDrop: (info) => {
      const event = info.event;

      const updated = {
        id: event.id,
        start: event.start,
        end: event.end
      };

      console.log('Rescheduled:', updated);
    },

    dayHeaderContent: (arg) => {
      const date = arg.date.toISOString().split('T')[0];

      const count = this.appointments
        .filter(a => a.date.startsWith(date)).length;

      return {
        html: `
          <div>
            <div>${arg.text}</div>
            ${count > 0 ? `<small style="color:#6c757d">${count} appt</small>` : ''}
          </div>
        `
      };
    },

    eventContent: (arg: any) => {
      const status = arg.event.extendedProps?.['status'] || '';

      const statusBg =
        status === 'Scheduled' ? '#d4edda' :
          status === 'Cancelled' ? '#f8d7da' :
            '#e2e3e5';

      return {
        html: `
          <div style="
            padding:8px;
            border-radius:8px;
            font-size:12px;
            background:white;
            box-shadow:0 2px 6px rgba(0,0,0,0.08);
          ">
            <div style="font-weight:600;">${arg.event.title}</div>
            <div style="opacity:0.7; margin-top:2px;">
              ${arg.timeText}
            </div>
            <div style="
              margin-top:6px;
              padding:2px 8px;
              border-radius:6px;
              background:${statusBg};
              display:inline-block;
              font-size:11px;
            ">
              ${status}
            </div>
          </div>
        `
      };
    },

    eventDidMount: (info) => {
      info.el.title = `
Patient: ${info.event.title}
Status: ${info.event.extendedProps['status']}
Notes: ${info.event.extendedProps['notes']}
      `;
    }
  };

  constructor(private appointmentService: AppointmentService) { }

  ngOnInit(): void {
    this.loadAppointments();
  }

  // ✅ Apply filter for selected day
  applyFilter() {
    if (!this.selectedDate) return;

    this.filteredAppointments = this.appointments.filter(a => {
      const sameDate = a.date.startsWith(this.selectedDate!);
      const statusMatch =
        this.statusFilter === 'All' || a.status === this.statusFilter;

      return sameDate && statusMatch;
    });
  }

  // ✅ Load data
  loadAppointments() {
    this.appointmentService.getMyAppointments().subscribe((res: any[]) => {

      // Sort latest first
      this.appointments = res.sort((a, b) =>
        new Date(b.date).getTime() - new Date(a.date).getTime()
      );

      const events = this.appointments.map(a => {
        const date = a.date.split('T')[0];

        return {
          id: a.id,
          title: a.patientName,
          start: `${date}T${a.startTime}`,
          end: `${date}T${a.endTime}`,

          backgroundColor: 'transparent',
          borderColor:
            a.status === 'Scheduled' ? '#28a745' :
              a.status === 'Cancelled' ? '#dc3545' :
                '#6c757d',

          textColor: '#000',

          extendedProps: {
            notes: a.notes,
            status: a.status
          }
        };
      });

      this.calendarOptions = {
        ...this.calendarOptions,
        events,
        eventDisplay: 'block',
        displayEventTime: false,
        displayEventEnd: false,

        dateClick: (info) => {
          this.selectedDate = info.dateStr;
          this.applyFilter();
        }
      };
    });
  }

  // ✅ Unified filter for LIST VIEW
  get filteredList() {
    return this.appointments.filter(a => {

      const matchesSearch =
        a.patientName.toLowerCase()
          .includes(this.searchText.toLowerCase());

      const matchesStatus =
        this.statusFilter === 'All' ||
        a.status === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }
}
