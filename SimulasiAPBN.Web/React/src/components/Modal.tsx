/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from 'react';

import './styles/Modal.css';

interface ModalProps {
    title: string;
    isOpen: boolean;
}

export const Modal: React.FC<ModalProps> = ({ title, isOpen, children }) => {
    const outsideRef = React.useRef(null);

    return isOpen ? (
        <div className={'modal'}>
            <div
                ref={outsideRef}
                className={'modal__overlay'}
            />
            <div className={'modal__box'}>
                <div className={'modal__title'}>
                    {title}
                </div>
                <div className={'modal__content'}>
                    {children}
                </div>
            </div>
        </div>
    ) : null;
};